using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.PurchaseOrder;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.PurchaseOrderManagement;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PurchaseOrderService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<PurchaseOrderDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        var purchaseOrders = await _context.PurchaseOrders
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .Include(x => x.PurchaseRequest)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return purchaseOrders
            .Select(MapToDto)
            .ToList();
    }


    public async Task<PurchaseOrderDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var purchaseOrder = await _context.PurchaseOrders
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .Include(x => x.PurchaseRequest)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        return MapToDto(purchaseOrder);
    }


    public async Task<PurchaseOrderDto> CreateAsync(
        CreatePurchaseOrderDto request)
    {
        var companyId = GetCompanyId();

        if (request.Items == null ||
            request.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one purchase order item is required.");
        }

        // Validate duplicate products
        var hasDuplicateProducts =
            request.Items
                .GroupBy(x => x.ProductId)
                .Any(x => x.Count() > 1);

        if (hasDuplicateProducts)
        {
            throw new InvalidOperationException(
                "Duplicate products are not allowed.");
        }

        // Validate quantities and prices
        if (request.Items.Any(x =>
            x.OrderedQuantity <= 0 ||
            x.UnitPrice < 0))
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero and unit price cannot be negative.");
        }

        // Validate Supplier
        var supplierExists =
            await _context.Suppliers
                .AnyAsync(x =>
                    x.Id == request.SupplierId &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (!supplierExists)
        {
            throw new KeyNotFoundException(
                "Supplier not found or inactive.");
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

        // Validate Products
        var productIds =
            request.Items
                .Select(x => x.ProductId)
                .ToList();

        var validProductCount =
            await _context.Products
                .CountAsync(x =>
                    productIds.Contains(x.Id) &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (validProductCount != productIds.Count)
        {
            throw new InvalidOperationException(
                "One or more products are invalid or inactive.");
        }

        // Validate Purchase Request if provided
        if (request.PurchaseRequestId.HasValue)
        {
            var purchaseRequest =
                await _context.PurchaseRequests
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.PurchaseRequestId.Value &&
                        x.CompanyId == companyId);

            if (purchaseRequest == null)
            {
                throw new KeyNotFoundException(
                    "Purchase request not found.");
            }

            if (purchaseRequest.Status !=
                PurchaseRequestStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only approved purchase requests can be used to create a purchase order.");
            }
        }

        // Calculate total
        var totalAmount =
            request.Items.Sum(x =>
                x.OrderedQuantity *
                x.UnitPrice);

        var purchaseOrder =
            new SupplyFlow.Domain.Entities.PurchaseOrder
            {
                CompanyId = companyId,

                PurchaseOrderNumber =
                    await GeneratePurchaseOrderNumberAsync(
                        companyId),

                SupplierId = request.SupplierId,

                WarehouseId = request.WarehouseId,

                PurchaseRequestId =
                    request.PurchaseRequestId,

                Status =
                    PurchaseOrderStatus.Draft,

                OrderDate =
                    DateTime.UtcNow,

                ExpectedDeliveryDate =
                    request.ExpectedDeliveryDate,

                TotalAmount =
                    totalAmount,

                Remarks =
                    request.Remarks?.Trim()
            };

        foreach (var item in request.Items)
        {
            var totalPrice =
                item.OrderedQuantity *
                item.UnitPrice;

            purchaseOrder.Items.Add(
                new SupplyFlow.Domain.Entities.PurchaseOrderItem
                {
                    ProductId =
                        item.ProductId,

                    OrderedQuantity =
                        item.OrderedQuantity,

                    UnitPrice =
                        item.UnitPrice,

                    TotalPrice =
                        totalPrice,

                    ReceivedQuantity = 0
                });
        }

        _context.PurchaseOrders
            .Add(purchaseOrder);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(
            purchaseOrder.Id);
    }


    public async Task<PurchaseOrderDto> SendAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var purchaseOrder =
            await _context.PurchaseOrders
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        if (purchaseOrder.Status !=
            PurchaseOrderStatus.Draft)
        {
            throw new InvalidOperationException(
                "Only draft purchase orders can be sent.");
        }

        purchaseOrder.Status =
            PurchaseOrderStatus.Sent;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    public async Task<PurchaseOrderDto> ConfirmAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var purchaseOrder =
            await _context.PurchaseOrders
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        if (purchaseOrder.Status !=
            PurchaseOrderStatus.Sent)
        {
            throw new InvalidOperationException(
                "Only sent purchase orders can be confirmed.");
        }

        purchaseOrder.Status =
            PurchaseOrderStatus.Confirmed;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    public async Task<PurchaseOrderDto> CancelAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var purchaseOrder =
            await _context.PurchaseOrders
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException(
                "Purchase order not found.");
        }

        if (purchaseOrder.Status ==
            PurchaseOrderStatus.Completed)
        {
            throw new InvalidOperationException(
                "Completed purchase orders cannot be cancelled.");
        }

        purchaseOrder.Status =
            PurchaseOrderStatus.Cancelled;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    private async Task<string>
        GeneratePurchaseOrderNumberAsync(
            int companyId)
    {
        var count =
            await _context.PurchaseOrders
                .CountAsync(x =>
                    x.CompanyId == companyId);

        return
            $"PO-{DateTime.UtcNow:yyyyMMdd}-{count + 1:D4}";
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


    private static PurchaseOrderDto MapToDto(
        SupplyFlow.Domain.Entities.PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,

            PurchaseOrderNumber =
                purchaseOrder.PurchaseOrderNumber,

            SupplierId =
                purchaseOrder.SupplierId,

            SupplierName =
                purchaseOrder.Supplier.Name,

            WarehouseId =
                purchaseOrder.WarehouseId,

            WarehouseName =
                purchaseOrder.Warehouse.Name,

            PurchaseRequestId =
                purchaseOrder.PurchaseRequestId,

            PurchaseRequestNumber =
                purchaseOrder
                    .PurchaseRequest?
                    .RequestNumber,

            Status =
                purchaseOrder.Status.ToString(),

            OrderDate =
                purchaseOrder.OrderDate,

            ExpectedDeliveryDate =
                purchaseOrder.ExpectedDeliveryDate,

            TotalAmount =
                purchaseOrder.TotalAmount,

            Remarks =
                purchaseOrder.Remarks,

            CreatedAt =
                purchaseOrder.CreatedAt,

            Items =
                purchaseOrder.Items
                    .Select(item =>
                        new PurchaseOrderItemDto
                        {
                            Id = item.Id,

                            ProductId =
                                item.ProductId,

                            ProductName =
                                item.Product.Name,

                            SKU =
                                item.Product.SKU,

                            OrderedQuantity =
                                item.OrderedQuantity,

                            UnitPrice =
                                item.UnitPrice,

                            TotalPrice =
                                item.TotalPrice,

                            ReceivedQuantity =
                                item.ReceivedQuantity
                        })
                    .ToList()
        };
    }
}