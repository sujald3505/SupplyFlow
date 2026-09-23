using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.GoodsReceipt;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Entities;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.GoodsReceiptManagement;

public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GoodsReceiptService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<GoodsReceiptDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        var goodsReceipts = await _context.GoodsReceipts
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.PurchaseOrder)
            .Include(x => x.Warehouse)
            .Include(x => x.ReceivedByUser)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return goodsReceipts
            .Select(MapToDto)
            .ToList();
    }


    public async Task<GoodsReceiptDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var goodsReceipt = await _context.GoodsReceipts
            .Include(x => x.PurchaseOrder)
            .Include(x => x.Warehouse)
            .Include(x => x.ReceivedByUser)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (goodsReceipt == null)
        {
            throw new KeyNotFoundException(
                "Goods receipt not found.");
        }

        return MapToDto(goodsReceipt);
    }


    public async Task<GoodsReceiptDto> CreateAsync(
        CreateGoodsReceiptDto request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        if (request.Items == null ||
            request.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one item is required.");
        }

        // Duplicate PO item check
        var duplicateItems = request.Items
            .GroupBy(x => x.PurchaseOrderItemId)
            .Any(x => x.Count() > 1);

        if (duplicateItems)
        {
            throw new InvalidOperationException(
                "Duplicate purchase order items are not allowed.");
        }

        // Validate received quantities
        if (request.Items.Any(x =>
            x.ReceivedQuantity <= 0))
        {
            throw new InvalidOperationException(
                "Received quantity must be greater than zero.");
        }

        // Validate Accepted + Rejected
        foreach (var item in request.Items)
        {
            var accepted =
                item.AcceptedQuantity ??
                item.ReceivedQuantity;

            var rejected =
                item.RejectedQuantity ?? 0;

            if (accepted < 0 || rejected < 0)
            {
                throw new InvalidOperationException(
                    "Accepted and rejected quantities cannot be negative.");
            }

            if (accepted + rejected !=
                item.ReceivedQuantity)
            {
                throw new InvalidOperationException(
                    "Accepted quantity plus rejected quantity must equal received quantity.");
            }
        }

        // Load Purchase Order
        var purchaseOrder =
            await _context.PurchaseOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.PurchaseOrderId &&
                    x.CompanyId == companyId);

        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        // Only confirmed / partially received PO
        if (purchaseOrder.Status !=
                PurchaseOrderStatus.Confirmed &&
            purchaseOrder.Status !=
                PurchaseOrderStatus.PartiallyReceived)
        {
            throw new InvalidOperationException(
                "Goods receipt can only be created for confirmed or partially received purchase orders.");
        }

        // Validate every PO item
        foreach (var requestItem in request.Items)
        {
            var poItem =
                purchaseOrder.Items
                    .FirstOrDefault(x =>
                        x.Id ==
                        requestItem.PurchaseOrderItemId);

            if (poItem == null)
            {
                throw new InvalidOperationException(
                    "Purchase order item does not belong to this purchase order.");
            }

            if (poItem.ProductId !=
                requestItem.ProductId)
            {
                throw new InvalidOperationException(
                    "Product does not match the purchase order item.");
            }

            var remainingQuantity =
                poItem.OrderedQuantity -
                poItem.ReceivedQuantity;

            if (requestItem.ReceivedQuantity >
                remainingQuantity)
            {
                throw new InvalidOperationException(
                    $"Received quantity exceeds remaining quantity for product ID {requestItem.ProductId}.");
            }
        }

        var goodsReceipt = new GoodsReceipt
        {
            CompanyId = companyId,

            GRNNumber =
                await GenerateGRNNumberAsync(
                    companyId),

            PurchaseOrderId =
                purchaseOrder.Id,

            WarehouseId =
                purchaseOrder.WarehouseId,

            ReceivedByUserId =
                userId,

            Status =
                GoodsReceiptStatus.Draft,

            ReceiptDate =
                request.ReceiptDate ??
                DateTime.UtcNow,

            Remarks =
                request.Remarks?.Trim()
        };

        foreach (var item in request.Items)
        {
            goodsReceipt.Items.Add(
                new GoodsReceiptItem
                {
                    ProductId =
                        item.ProductId,

                    PurchaseOrderItemId =
                        item.PurchaseOrderItemId,

                    ReceivedQuantity =
                        item.ReceivedQuantity,

                    AcceptedQuantity =
                        item.AcceptedQuantity ??
                        item.ReceivedQuantity,

                    RejectedQuantity =
                        item.RejectedQuantity ?? 0,

                    Remarks =
                        item.Remarks?.Trim()
                });
        }

        _context.GoodsReceipts.Add(
            goodsReceipt);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(
            goodsReceipt.Id);
    }


    public async Task<GoodsReceiptDto> CompleteAsync(
        int id)
    {
        var companyId = GetCompanyId();

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync();

        try
        {
            var goodsReceipt =
                await _context.GoodsReceipts
                    .Include(x => x.PurchaseOrder)
                        .ThenInclude(x => x.Items)
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.CompanyId == companyId);

            if (goodsReceipt == null)
            {
                throw new KeyNotFoundException(
                    "Goods receipt not found.");
            }

            if (goodsReceipt.Status !=
                GoodsReceiptStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only draft goods receipts can be completed.");
            }

            var purchaseOrder =
                goodsReceipt.PurchaseOrder;

            // Re-validate PO status
            if (purchaseOrder.Status !=
                    PurchaseOrderStatus.Confirmed &&
                purchaseOrder.Status !=
                    PurchaseOrderStatus.PartiallyReceived)
            {
                throw new InvalidOperationException(
                    "Purchase order is not available for receiving.");
            }

            foreach (var grnItem in
                goodsReceipt.Items)
            {
                var poItem =
                    purchaseOrder.Items
                        .First(x =>
                            x.Id ==
                            grnItem.PurchaseOrderItemId);

                var remainingQuantity =
                    poItem.OrderedQuantity -
                    poItem.ReceivedQuantity;

                if (grnItem.ReceivedQuantity >
                    remainingQuantity)
                {
                    throw new InvalidOperationException(
                        "Received quantity exceeds remaining quantity.");
                }

                // Update PO received quantity
                poItem.ReceivedQuantity +=
                    grnItem.ReceivedQuantity;

                // Only accepted quantity goes into stock
                var acceptedQuantity =
                    grnItem.AcceptedQuantity ??
                    grnItem.ReceivedQuantity;

                // Get Warehouse Stock
                var warehouseStock =
                    await _context.WarehouseStocks
                        .FirstOrDefaultAsync(x =>
                            x.CompanyId == companyId &&
                            x.WarehouseId ==
                                goodsReceipt.WarehouseId &&
                            x.ProductId ==
                                grnItem.ProductId);

                var previousQuantity =
                    warehouseStock?.Quantity ?? 0;

                var newQuantity =
                    previousQuantity +
                    acceptedQuantity;

                if (warehouseStock == null)
                {
                    warehouseStock =
                        new WarehouseStock
                        {
                            CompanyId = companyId,

                            WarehouseId =
                                goodsReceipt.WarehouseId,

                            ProductId =
                                grnItem.ProductId,

                            Quantity =
                                newQuantity
                        };

                    _context.WarehouseStocks
                        .Add(warehouseStock);
                }
                else
                {
                    warehouseStock.Quantity =
                        newQuantity;
                }

                // Inventory Transaction
                _context.InventoryTransactions
                    .Add(
                        new InventoryTransaction
                        {
                            CompanyId = companyId,

                            WarehouseId =
                                goodsReceipt.WarehouseId,

                            ProductId =
                                grnItem.ProductId,

                            TransactionType =
                                InventoryTransactionType.StockIn,

                            Quantity =
                                acceptedQuantity,

                            PreviousQuantity =
                                previousQuantity,

                            NewQuantity =
                                newQuantity,

                            ReferenceNumber =
                                goodsReceipt.GRNNumber,

                            Remarks =
                                $"Goods received from PO: {purchaseOrder.PurchaseOrderNumber}"
                        });
            }

            // Check PO completion
            var isCompleted =
                purchaseOrder.Items
                    .All(x =>
                        x.ReceivedQuantity >=
                        x.OrderedQuantity);

            purchaseOrder.Status =
                isCompleted
                    ? PurchaseOrderStatus.Completed
                    : PurchaseOrderStatus.PartiallyReceived;

            goodsReceipt.Status =
                GoodsReceiptStatus.Completed;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetByIdAsync(id);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<GoodsReceiptDto> CancelAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var goodsReceipt =
            await _context.GoodsReceipts
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (goodsReceipt == null)
        {
            throw new KeyNotFoundException(
                "Goods receipt not found.");
        }

        if (goodsReceipt.Status !=
            GoodsReceiptStatus.Draft)
        {
            throw new InvalidOperationException(
                "Only draft goods receipts can be cancelled.");
        }

        goodsReceipt.Status =
            GoodsReceiptStatus.Cancelled;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    private async Task<string>
        GenerateGRNNumberAsync(
            int companyId)
    {
        var count =
            await _context.GoodsReceipts
                .CountAsync(x =>
                    x.CompanyId == companyId);

        return
            $"GRN-{DateTime.UtcNow:yyyyMMdd}-{count + 1:D4}";
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


    private int GetUserId()
    {
        if (!_currentUser.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User information not found.");
        }

        return _currentUser.UserId.Value;
    }


    private static GoodsReceiptDto MapToDto(
        GoodsReceipt goodsReceipt)
    {
        return new GoodsReceiptDto
        {
            Id = goodsReceipt.Id,

            GRNNumber =
                goodsReceipt.GRNNumber,

            PurchaseOrderId =
                goodsReceipt.PurchaseOrderId,

            PurchaseOrderNumber =
                goodsReceipt
                    .PurchaseOrder
                    .PurchaseOrderNumber,

            WarehouseId =
                goodsReceipt.WarehouseId,

            WarehouseName =
                goodsReceipt
                    .Warehouse
                    .Name,

            ReceivedByUserId =
                goodsReceipt.ReceivedByUserId,

            ReceivedByUserName =
                goodsReceipt
                    .ReceivedByUser
                    .FullName,

            Status =
                goodsReceipt.Status
                    .ToString(),

            ReceiptDate =
                goodsReceipt.ReceiptDate,

            Remarks =
                goodsReceipt.Remarks,

            CreatedAt =
                goodsReceipt.CreatedAt,

            Items =
                goodsReceipt.Items
                    .Select(item =>
                        new GoodsReceiptItemDto
                        {
                            Id = item.Id,

                            ProductId =
                                item.ProductId,

                            ProductName =
                                item.Product.Name,

                            PurchaseOrderItemId =
                                item.PurchaseOrderItemId,

                            ReceivedQuantity =
                                item.ReceivedQuantity,

                            AcceptedQuantity =
                                item.AcceptedQuantity,

                            RejectedQuantity =
                                item.RejectedQuantity,

                            Remarks =
                                item.Remarks
                        })
                    .ToList()
        };
    }
}