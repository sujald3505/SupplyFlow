using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.InventoryTransaction;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.InventoryManagement;

public class InventoryTransactionService
    : IInventoryTransactionService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public InventoryTransactionService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<InventoryTransactionDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.InventoryTransactions
            .Where(x => x.CompanyId == companyId)
            .OrderByDescending(x => x.Id)
            .Select(x => new InventoryTransactionDto
            {
                Id = x.Id,

                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                TransactionType =
                    x.TransactionType.ToString(),

                Quantity = x.Quantity,
                PreviousQuantity = x.PreviousQuantity,
                NewQuantity = x.NewQuantity,

                ReferenceNumber =
                    x.ReferenceNumber,

                Remarks = x.Remarks,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<List<InventoryTransactionDto>>
        GetByWarehouseIdAsync(int warehouseId)
    {
        var companyId = GetCompanyId();

        var warehouseExists = await _context.Warehouses
            .AnyAsync(x =>
                x.Id == warehouseId &&
                x.CompanyId == companyId);

        if (!warehouseExists)
        {
            throw new KeyNotFoundException(
                "Warehouse not found.");
        }

        return await _context.InventoryTransactions
            .Where(x =>
                x.CompanyId == companyId &&
                x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.Id)
            .Select(x => new InventoryTransactionDto
            {
                Id = x.Id,

                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                TransactionType =
                    x.TransactionType.ToString(),

                Quantity = x.Quantity,
                PreviousQuantity =
                    x.PreviousQuantity,
                NewQuantity =
                    x.NewQuantity,

                ReferenceNumber =
                    x.ReferenceNumber,

                Remarks = x.Remarks,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<List<InventoryTransactionDto>>
        GetByProductIdAsync(int productId)
    {
        var companyId = GetCompanyId();

        var productExists = await _context.Products
            .AnyAsync(x =>
                x.Id == productId &&
                x.CompanyId == companyId);

        if (!productExists)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        return await _context.InventoryTransactions
            .Where(x =>
                x.CompanyId == companyId &&
                x.ProductId == productId)
            .OrderByDescending(x => x.Id)
            .Select(x => new InventoryTransactionDto
            {
                Id = x.Id,

                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                TransactionType =
                    x.TransactionType.ToString(),

                Quantity = x.Quantity,
                PreviousQuantity =
                    x.PreviousQuantity,
                NewQuantity =
                    x.NewQuantity,

                ReferenceNumber =
                    x.ReferenceNumber,

                Remarks = x.Remarks,

                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<InventoryTransactionDto> CreateAsync(
        CreateInventoryTransactionDto request)
    {
        var companyId = GetCompanyId();

        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Transaction quantity must be greater than zero.");
        }

        // Validate Warehouse
        var warehouseExists =
            await _context.Warehouses
                .AnyAsync(x =>
                    x.Id == request.WarehouseId &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (!warehouseExists)
        {
            throw new KeyNotFoundException(
                "Warehouse not found or inactive.");
        }

        // Validate Product
        var productExists =
            await _context.Products
                .AnyAsync(x =>
                    x.Id == request.ProductId &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (!productExists)
        {
            throw new KeyNotFoundException(
                "Product not found or inactive.");
        }

        // Find existing warehouse stock
        var stock = await _context.WarehouseStocks
            .FirstOrDefaultAsync(x =>
                x.CompanyId == companyId &&
                x.WarehouseId == request.WarehouseId &&
                x.ProductId == request.ProductId);

        decimal previousQuantity = 0;

        if (stock != null)
        {
            previousQuantity = stock.Quantity;
        }

        decimal newQuantity;

        switch (request.TransactionType)
        {
            case InventoryTransactionType.StockIn:

                newQuantity =
                    previousQuantity + request.Quantity;

                break;


            case InventoryTransactionType.StockOut:

                newQuantity =
                    previousQuantity - request.Quantity;

                if (newQuantity < 0)
                {
                    throw new InvalidOperationException(
                        "Insufficient stock. Negative stock is not allowed.");
                }

                break;


            case InventoryTransactionType.Adjustment:

                // Positive adjustment
                newQuantity =
                    previousQuantity + request.Quantity;

                break;


            default:

                throw new InvalidOperationException(
                    "Invalid transaction type.");
        }

        // Create stock if it does not exist
        if (stock == null)
        {
            stock =
                new SupplyFlow.Domain.Entities.WarehouseStock
                {
                    CompanyId = companyId,
                    WarehouseId = request.WarehouseId,
                    ProductId = request.ProductId,
                    Quantity = newQuantity
                };

            _context.WarehouseStocks.Add(stock);
        }
        else
        {
            stock.Quantity = newQuantity;
        }

        // Create transaction
        var transaction =
            new SupplyFlow.Domain.Entities.InventoryTransaction
            {
                CompanyId = companyId,
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId,

                TransactionType =
                    request.TransactionType,

                Quantity = request.Quantity,

                PreviousQuantity =
                    previousQuantity,

                NewQuantity =
                    newQuantity,

                ReferenceNumber =
                    request.ReferenceNumber?.Trim(),

                Remarks =
                    request.Remarks?.Trim()
            };

        _context.InventoryTransactions
            .Add(transaction);

        await _context.SaveChangesAsync();

        await _context.Entry(transaction)
            .Reference(x => x.Warehouse)
            .LoadAsync();

        await _context.Entry(transaction)
            .Reference(x => x.Product)
            .LoadAsync();

        return MapToDto(transaction);
    }


    private int GetCompanyId()
    {
        if (!_currentUser.CompanyId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Company information not found.");
        }

        return _currentUser.CompanyId.Value;
    }


    private static InventoryTransactionDto MapToDto(
        SupplyFlow.Domain.Entities.InventoryTransaction transaction)
    {
        return new InventoryTransactionDto
        {
            Id = transaction.Id,

            WarehouseId =
                transaction.WarehouseId,

            WarehouseName =
                transaction.Warehouse.Name,

            ProductId =
                transaction.ProductId,

            ProductName =
                transaction.Product.Name,

            SKU =
                transaction.Product.SKU,

            TransactionType =
                transaction.TransactionType.ToString(),

            Quantity =
                transaction.Quantity,

            PreviousQuantity =
                transaction.PreviousQuantity,

            NewQuantity =
                transaction.NewQuantity,

            ReferenceNumber =
                transaction.ReferenceNumber,

            Remarks =
                transaction.Remarks,

            CreatedAt =
                transaction.CreatedAt
        };
    }
}