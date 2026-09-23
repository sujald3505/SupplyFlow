using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Sale;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Entities;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.SaleManagement;

public class SaleService : ISaleService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SaleService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<SaleDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        var sales = await _context.Sales
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.SaleDate)
            .ToListAsync();

        return sales
            .Select(MapToDto)
            .ToList();
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var companyId = GetCompanyId();

        var sale = await _context.Sales
            .Where(x =>
                x.Id == id &&
                x.CompanyId == companyId)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync();

        if (sale == null)
        {
            return null;
        }

        return MapToDto(sale);
    }

    public async Task<SaleDto> CreateAsync(
        CreateSaleDto request)
    {
        var companyId = GetCompanyId();

        if (request.Items == null ||
            request.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one product is required.");
        }

        if (request.Items.Any(x =>
            x.ProductId <= 0 ||
            x.Quantity <= 0))
        {
            throw new InvalidOperationException(
                "Product and quantity must be valid.");
        }

        // =========================
        // VALIDATE WAREHOUSE
        // =========================

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(x =>
                x.Id == request.WarehouseId &&
                x.CompanyId == companyId &&
                x.IsActive);

        if (warehouse == null)
        {
            throw new KeyNotFoundException(
                "Warehouse not found or inactive.");
        }

        // =========================
        // LOAD PRODUCTS
        // =========================

        var productIds = request.Items
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Where(x =>
                productIds.Contains(x.Id) &&
                x.CompanyId == companyId &&
                x.IsActive)
            .ToListAsync();

        if (products.Count != productIds.Count)
        {
            throw new KeyNotFoundException(
                "One or more products were not found or inactive.");
        }

        // =========================
        // CREATE INVOICE NUMBER
        // =========================

        var invoiceNumber =
            await GenerateInvoiceNumberAsync(companyId);

        // =========================
        // CREATE SALE
        // =========================

        var sale = new Sale
        {
            CompanyId = companyId,

            WarehouseId =
                request.WarehouseId,

            CustomerId =
                request.CustomerId,

            InvoiceNumber =
                invoiceNumber,

            SaleDate =
                DateTime.UtcNow,

            Discount =
                request.Discount,

            Tax =
                request.Tax,

            Remarks =
                request.Remarks?.Trim(),

            Status = "Completed"
        };

        decimal subTotal = 0;

        // =========================
        // PROCESS ITEMS + STOCK
        // =========================

        foreach (var requestItem in request.Items)
        {
            var product = products
                .First(x =>
                    x.Id == requestItem.ProductId);

            // Find warehouse stock
            var stock = await _context.WarehouseStocks
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == companyId &&
                    x.WarehouseId ==
                        request.WarehouseId &&
                    x.ProductId ==
                        requestItem.ProductId);

            if (stock == null)
            {
                throw new InvalidOperationException(
                    $"No stock found for product '{product.Name}'.");
            }

            // Check available stock
            if (stock.Quantity <
                requestItem.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. " +
                    $"Available: {stock.Quantity}, " +
                    $"Requested: {requestItem.Quantity}.");
            }

            // =========================
            // CALCULATE ITEM TOTAL
            // =========================

            var grossAmount =
                requestItem.Quantity *
                requestItem.UnitPrice;

            var itemTotal =
                grossAmount -
                requestItem.Discount +
                requestItem.Tax;

            if (itemTotal < 0)
            {
                throw new InvalidOperationException(
                    $"Invalid total for product '{product.Name}'.");
            }

            subTotal += itemTotal;

            // =========================
            // STOCK BEFORE
            // =========================

            var previousQuantity =
                stock.Quantity;

            // =========================
            // STOCK MINUS
            // =========================

            stock.Quantity -=
                requestItem.Quantity;

            var newQuantity =
                stock.Quantity;

            // =========================
            // INVENTORY TRANSACTION
            // =========================

            var transaction =
                new InventoryTransaction
                {
                    CompanyId =
                        companyId,

                    WarehouseId =
                        request.WarehouseId,

                    ProductId =
                        requestItem.ProductId,

                    TransactionType =
                        InventoryTransactionType.StockOut,

                    Quantity =
                        requestItem.Quantity,

                    PreviousQuantity =
                        previousQuantity,

                    NewQuantity =
                        newQuantity,

                    ReferenceNumber =
                        invoiceNumber,

                    Remarks =
                        "Sale"
                };

            _context.InventoryTransactions
                .Add(transaction);

            // =========================
            // SALE ITEM
            // =========================

            var saleItem =
                new SaleItem
                {
                    ProductId =
                        requestItem.ProductId,

                    Quantity =
                        requestItem.Quantity,

                    UnitPrice =
                        requestItem.UnitPrice,

                    Discount =
                        requestItem.Discount,

                    Tax =
                        requestItem.Tax,

                    Total =
                        itemTotal
                };

            sale.Items.Add(saleItem);
        }

        // =========================
        // SALE TOTALS
        // =========================

        sale.SubTotal =
            subTotal;

        sale.GrandTotal =
            subTotal -
            sale.Discount +
            sale.Tax;

        if (sale.GrandTotal < 0)
        {
            throw new InvalidOperationException(
                "Grand total cannot be negative.");
        }

        // =========================
        // SAVE EVERYTHING
        // =========================

        _context.Sales.Add(sale);

        await _context.SaveChangesAsync();

        // =========================
        // LOAD RESPONSE DATA
        // =========================

        await _context.Entry(sale)
            .Reference(x => x.Warehouse)
            .LoadAsync();

        foreach (var item in sale.Items)
        {
            await _context.Entry(item)
                .Reference(x => x.Product)
                .LoadAsync();
        }

        return MapToDto(sale);
    }

    // =========================
    // INVOICE NUMBER
    // =========================

    private async Task<string>
        GenerateInvoiceNumberAsync(int companyId)
    {
        var datePart =
            DateTime.UtcNow
                .ToString("yyyyMMdd");

        var prefix =
            $"INV-{datePart}-";

        var count =
            await _context.Sales
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    x.InvoiceNumber.StartsWith(prefix));

        return $"{prefix}{count + 1:D4}";
    }

    // =========================
    // COMPANY ID
    // =========================

    private int GetCompanyId()
    {
        if (!_currentUser.CompanyId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Company information not found.");
        }

        return _currentUser.CompanyId.Value;
    }

    // =========================
    // MAPPING
    // =========================

    private static SaleDto MapToDto(
        Sale sale)
    {
        return new SaleDto
        {
            Id =
                sale.Id,

            InvoiceNumber =
                sale.InvoiceNumber,

            WarehouseId =
                sale.WarehouseId,

            WarehouseName =
                sale.Warehouse?.Name ?? string.Empty,

            CustomerId =
                sale.CustomerId,

            SaleDate =
                sale.SaleDate,

            SubTotal =
                sale.SubTotal,

            Discount =
                sale.Discount,

            Tax =
                sale.Tax,

            GrandTotal =
                sale.GrandTotal,

            Status =
                sale.Status,

            Remarks =
                sale.Remarks,

            Items =
                sale.Items
                    .Select(item => new SaleItemDto
                    {
                        Id =
                            item.Id,

                        ProductId =
                            item.ProductId,

                        ProductName =
                            item.Product?.Name
                            ?? string.Empty,

                        SKU =
                            item.Product?.SKU
                            ?? string.Empty,

                        Quantity =
                            item.Quantity,

                        UnitPrice =
                            item.UnitPrice,

                        Discount =
                            item.Discount,

                        Tax =
                            item.Tax,

                        Total =
                            item.Total
                    })
                    .ToList()
        };
    }
}