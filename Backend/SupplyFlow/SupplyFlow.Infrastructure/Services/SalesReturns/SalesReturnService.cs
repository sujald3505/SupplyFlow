using Microsoft.EntityFrameworkCore;

using SupplyFlow.Application.DTOs.SalesReturn;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Entities;
using SupplyFlow.Infrastructure.Persistence;

// Entity aliases - avoids namespace/class conflict
using SalesReturnEntity =
    SupplyFlow.Domain.Entities.SalesReturn;

using SalesReturnItemEntity =
    SupplyFlow.Domain.Entities.SalesReturnItem;

namespace SupplyFlow.Infrastructure.Services.SalesReturn;

public class SalesReturnService : ISalesReturnService
{
    private readonly SupplyFlowDbContext _context;

    public SalesReturnService(
        SupplyFlowDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<List<SalesReturnDto>> GetAllAsync()
    {
        return await _context.SalesReturns
            .AsNoTracking()
            .Include(x => x.Sale)
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.ReturnDate)
            .Select(x => new SalesReturnDto
            {
                Id = x.Id,

                ReturnNumber = x.ReturnNumber,

                SaleId = x.SaleId,

                InvoiceNumber = x.Sale.InvoiceNumber,

                WarehouseId = x.WarehouseId,

                WarehouseName = x.Warehouse.Name,

                CustomerId = x.CustomerId,

                CustomerName = x.Customer.Name,

                ReturnDate = x.ReturnDate,

                SubTotal = x.SubTotal,

                Discount = x.Discount,

                Tax = x.Tax,

                GrandTotal = x.GrandTotal,

                Status = x.Status,

                Remarks = x.Remarks,

                Items = x.Items
                    .Select(item => new SalesReturnItemDto
                    {
                        Id = item.Id,

                        SaleItemId = item.SaleItemId,

                        ProductId = item.ProductId,

                        ProductName = item.Product.Name,

                        SKU = item.Product.SKU,

                        Quantity = item.Quantity,

                        UnitPrice = item.UnitPrice,

                        Discount = item.Discount,

                        Tax = item.Tax,

                        Total = item.Total
                    })
                    .ToList()
            })
            .ToListAsync();
    }


    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<SalesReturnDto?> GetByIdAsync(int id)
    {
        return await _context.SalesReturns
            .AsNoTracking()
            .Include(x => x.Sale)
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new SalesReturnDto
            {
                Id = x.Id,

                ReturnNumber = x.ReturnNumber,

                SaleId = x.SaleId,

                InvoiceNumber = x.Sale.InvoiceNumber,

                WarehouseId = x.WarehouseId,

                WarehouseName = x.Warehouse.Name,

                CustomerId = x.CustomerId,

                CustomerName = x.Customer.Name,

                ReturnDate = x.ReturnDate,

                SubTotal = x.SubTotal,

                Discount = x.Discount,

                Tax = x.Tax,

                GrandTotal = x.GrandTotal,

                Status = x.Status,

                Remarks = x.Remarks,

                Items = x.Items
                    .Select(item => new SalesReturnItemDto
                    {
                        Id = item.Id,

                        SaleItemId = item.SaleItemId,

                        ProductId = item.ProductId,

                        ProductName = item.Product.Name,

                        SKU = item.Product.SKU,

                        Quantity = item.Quantity,

                        UnitPrice = item.UnitPrice,

                        Discount = item.Discount,

                        Tax = item.Tax,

                        Total = item.Total
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }


    // =========================================================
    // CREATE SALES RETURN
    // =========================================================

    public async Task<SalesReturnDto> CreateAsync(
        CreateSalesReturnDto request)
    {
        if (request.Items == null ||
            request.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one item is required.");
        }


        // =====================================================
        // FIND SALE
        // =====================================================

        var sale = await _context.Sales
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == request.SaleId);

        if (sale == null)
        {
            throw new InvalidOperationException(
                "Sale not found.");
        }


        // =====================================================
        // VALIDATE WAREHOUSE
        // =====================================================

        if (sale.WarehouseId != request.WarehouseId)
        {
            throw new InvalidOperationException(
                "Warehouse does not belong to selected sale.");
        }


        // =====================================================
        // VALIDATE CUSTOMER
        // =====================================================

        if (sale.CustomerId != request.CustomerId)
        {
            throw new InvalidOperationException(
                "Customer does not belong to selected sale.");
        }


        // =====================================================
        // GET PREVIOUS RETURNS
        // =====================================================

        var previousReturns =
            await _context.SalesReturnItems
                .Where(x =>
                    x.SalesReturn.SaleId ==
                    request.SaleId)
                .GroupBy(x => x.SaleItemId)
                .Select(x => new
                {
                    SaleItemId = x.Key,

                    Quantity =
                        x.Sum(y => y.Quantity)
                })
                .ToDictionaryAsync(
                    x => x.SaleItemId,
                    x => x.Quantity);


        // =====================================================
        // RETURN ITEMS
        // =====================================================

        var returnItems =
            new List<SalesReturnItemEntity>();


        // =====================================================
        // VALIDATE EACH ITEM
        // =====================================================

        foreach (var requestItem in request.Items)
        {
            if (requestItem.Quantity <= 0)
            {
                throw new InvalidOperationException(
                    "Return quantity must be greater than zero.");
            }


            var saleItem =
                sale.Items.FirstOrDefault(
                    x =>
                        x.Id ==
                        requestItem.SaleItemId);


            if (saleItem == null)
            {
                throw new InvalidOperationException(
                    $"Sale item {requestItem.SaleItemId} not found.");
            }


            if (saleItem.ProductId !=
                requestItem.ProductId)
            {
                throw new InvalidOperationException(
                    "Product does not match the original sale item.");
            }


            var alreadyReturned =
                previousReturns.TryGetValue(
                    saleItem.Id,
                    out var returnedQuantity)
                    ? returnedQuantity
                    : 0;


            var remainingQuantity =
                saleItem.Quantity -
                alreadyReturned;


            if (requestItem.Quantity >
                remainingQuantity)
            {
                throw new InvalidOperationException(
                    $"Return quantity cannot exceed " +
                    $"remaining quantity {remainingQuantity}.");
            }


            // =================================================
            // PRICE
            // =================================================

            var unitPrice =
                saleItem.UnitPrice;


            var gross =
                requestItem.Quantity *
                unitPrice;


            var requestedDiscount =
                requestItem.Discount;


            var itemDiscount =
                Math.Min(
                    Math.Max(
                        requestedDiscount,
                        0),
                    gross);


            var taxableAmount =
                gross -
                itemDiscount;


            var taxRate =
                Math.Max(
                    requestItem.Tax,
                    0);


            var itemTax =
                taxableAmount *
                taxRate /
                100m;


            var total =
                taxableAmount +
                itemTax;


            returnItems.Add(
                new SalesReturnItemEntity
                {
                    SaleItemId =
                        saleItem.Id,

                    ProductId =
                        saleItem.ProductId,

                    Quantity =
                        requestItem.Quantity,

                    UnitPrice =
                        unitPrice,

                    Discount =
                        itemDiscount,

                    Tax =
                        itemTax,

                    Total =
                        total
                });
        }


        // =====================================================
        // TOTALS
        // =====================================================

        var subTotal =
            returnItems.Sum(
                x =>
                    x.Quantity *
                    x.UnitPrice);


        var totalDiscount =
            returnItems.Sum(
                x => x.Discount);


        var totalTax =
            returnItems.Sum(
                x => x.Tax);


        var grandTotal =
            subTotal -
            totalDiscount +
            totalTax;


        // =====================================================
        // RETURN NUMBER
        // =====================================================

        var returnNumber =
            $"SR-{DateTime.UtcNow:ddmmyyyyHHmmssfff}";


        // =====================================================
        // CREATE RETURN ENTITY
        // =====================================================

        var salesReturn =
            new SalesReturnEntity
            {
                CompanyId =
                    sale.CompanyId,

                SaleId =
                    sale.Id,

                WarehouseId =
                    sale.WarehouseId,

                CustomerId =
                    sale.CustomerId,

                ReturnNumber =
                    returnNumber,

                ReturnDate =
                    DateTime.UtcNow,

                SubTotal =
                    subTotal,

                Discount =
                    totalDiscount,

                Tax =
                    totalTax,

                GrandTotal =
                    grandTotal,

                Status =
                    "Completed",

                Remarks =
                    request.Remarks,

                Items =
                    returnItems
            };


        // =====================================================
        // DATABASE TRANSACTION
        // =====================================================

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync();


        try
        {
            // =================================================
            // SAVE SALES RETURN
            // =================================================

            _context.SalesReturns.Add(
                salesReturn);

            await _context.SaveChangesAsync();


            // =================================================
            // UPDATE STOCK
            // =================================================

            foreach (var item in returnItems)
            {
                var stock =
                    await _context.WarehouseStocks
                        .FirstOrDefaultAsync(
                            x =>
                                x.WarehouseId ==
                                sale.WarehouseId
                                &&
                                x.ProductId ==
                                item.ProductId);


                if (stock == null)
                {
                    throw new InvalidOperationException(
                        $"Stock record not found for " +
                        $"product ID {item.ProductId}.");
                }


                var previousQuantity =
                    stock.Quantity;


                stock.Quantity +=
                    item.Quantity;


                // =============================================
                // INVENTORY TRANSACTION
                // =============================================

                var inventoryTransaction =
                    new InventoryTransaction
                    {

                        CompanyId = sale.CompanyId,

                        WarehouseId =
                            sale.WarehouseId,

                        ProductId =
                            item.ProductId,

                        Quantity =
                            item.Quantity,

                        PreviousQuantity =
                            previousQuantity,

                        NewQuantity =
                            stock.Quantity,

                        ReferenceNumber =
                            returnNumber,

                        Remarks =
                            $"Sales return against invoice " +
                            $"{sale.InvoiceNumber}"
                    };


                _context.InventoryTransactions.Add(
                    inventoryTransaction);
            }


            // =================================================
            // SAVE STOCK + TRANSACTIONS
            // =================================================

            await _context.SaveChangesAsync();


            // =================================================
            // COMMIT
            // =================================================

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }


        // =====================================================
        // RETURN CREATED RECORD
        // =====================================================

        var createdReturn =
            await GetByIdAsync(
                salesReturn.Id);


        if (createdReturn == null)
        {
            throw new InvalidOperationException(
                "Failed to load created sales return.");
        }


        return createdReturn;
    }
}