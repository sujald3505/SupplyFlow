using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Dashboard;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DashboardService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var companyId = GetCompanyId();

        // ============================
        // PRODUCTS
        // ============================

        var totalProducts =
            await _context.Products
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted);

        var activeProducts =
            await _context.Products
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    x.IsActive &&
                    !x.IsDeleted);


        // ============================
        // WAREHOUSES
        // ============================

        var totalWarehouses =
            await _context.Warehouses
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted);

        var activeWarehouses =
            await _context.Warehouses
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    x.IsActive &&
                    !x.IsDeleted);


        // ============================
        // SUPPLIERS
        // ============================

        var totalSuppliers =
            await _context.Suppliers
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted);

        var activeSuppliers =
            await _context.Suppliers
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    x.IsActive &&
                    !x.IsDeleted);


        // ============================
        // INVENTORY
        // ============================

        var totalStockQuantity =
            await _context.WarehouseStocks
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted)
                .SumAsync(x =>
                    (decimal?)x.Quantity) ?? 0;


        var totalInventoryValue =
            await _context.WarehouseStocks
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    !x.Product.IsDeleted)
                .SumAsync(x =>
                    (decimal?)(x.Quantity *
                               x.Product.CostPrice)) ?? 0;


        // ============================
        // LOW STOCK
        // ============================

        var lowStockProducts =
            await _context.WarehouseStocks
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    !x.Product.IsDeleted &&
                    x.Quantity <
                    x.Product.MinimumStockLevel)
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .OrderBy(x => x.Quantity)
                .Select(x =>
                    new LowStockDashboardDto
                    {
                        ProductId =
                            x.ProductId,

                        ProductName =
                            x.Product.Name,

                        SKU =
                            x.Product.SKU,

                        WarehouseId =
                            x.WarehouseId,

                        WarehouseName =
                            x.Warehouse.Name,

                        CurrentQuantity =
                            x.Quantity,

                        MinimumStockLevel =
                            x.Product.MinimumStockLevel,

                        ShortageQuantity =
                            x.Product.MinimumStockLevel -
                            x.Quantity
                    })
                .Take(10)
                .ToListAsync();


        // ============================
        // PURCHASE REQUESTS
        // ============================

        var draftPurchaseRequests =
            await _context.PurchaseRequests
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    x.Status ==
                    PurchaseRequestStatus.Draft);

        var submittedPurchaseRequests =
            await _context.PurchaseRequests
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    x.Status ==
                    PurchaseRequestStatus.Submitted);


        // ============================
        // PURCHASE ORDERS
        // ============================

        var draftPurchaseOrders =
            await _context.PurchaseOrders
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    x.Status ==
                    PurchaseOrderStatus.Draft);

        var confirmedPurchaseOrders =
            await _context.PurchaseOrders
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    x.Status ==
                    PurchaseOrderStatus.Confirmed);


        // ============================
        // GOODS RECEIPTS
        // ============================

        var completedGoodsReceipts =
            await _context.GoodsReceipts
                .CountAsync(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    x.Status ==
                    GoodsReceiptStatus.Completed);


        // ============================
        // RECENT PURCHASE REQUESTS
        // ============================

        var recentPurchaseRequests =
            await _context.PurchaseRequests
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted)
                .Include(x => x.RequestedByUser)
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x =>
                    new RecentPurchaseRequestDto
                    {
                        Id = x.Id,

                        RequestNumber =
                            x.RequestNumber,

                        RequestedBy =
                            x.RequestedByUser.FullName,

                        Status =
                            x.Status.ToString(),

                        CreatedAt =
                            x.CreatedAt
                    })
                .ToListAsync();


        // ============================
        // RECENT PURCHASE ORDERS
        // ============================

        var recentPurchaseOrders =
            await _context.PurchaseOrders
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted)
                .Include(x => x.Supplier)
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x =>
                    new RecentPurchaseOrderDto
                    {
                        Id = x.Id,

                        PurchaseOrderNumber =
                            x.PurchaseOrderNumber,

                        SupplierName =
                            x.Supplier.Name,

                        Status =
                            x.Status.ToString(),

                        TotalAmount =
                            x.TotalAmount,

                        CreatedAt =
                            x.CreatedAt
                    })
                .ToListAsync();


        // ============================
        // RECENT GOODS RECEIPTS
        // ============================

        var recentGoodsReceipts =
            await _context.GoodsReceipts
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted)
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Warehouse)
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x =>
                    new RecentGoodsReceiptDto
                    {
                        Id = x.Id,

                        GRNNumber =
                            x.GRNNumber,

                        PurchaseOrderNumber =
                            x.PurchaseOrder
                                .PurchaseOrderNumber,

                        WarehouseName =
                            x.Warehouse.Name,

                        Status =
                            x.Status.ToString(),

                        ReceiptDate =
                            x.ReceiptDate
                    })
                .ToListAsync();


        // ============================
        // FINAL RESPONSE
        // ============================

        return new DashboardDto
        {
            Summary =
                new DashboardSummaryDto
                {
                    TotalProducts =
                        totalProducts,

                    ActiveProducts =
                        activeProducts,

                    TotalWarehouses =
                        totalWarehouses,

                    ActiveWarehouses =
                        activeWarehouses,

                    TotalSuppliers =
                        totalSuppliers,

                    ActiveSuppliers =
                        activeSuppliers,

                    TotalStockQuantity =
                        totalStockQuantity,

                    TotalInventoryValue =
                        totalInventoryValue,

                    LowStockProducts =
                        lowStockProducts.Count,

                    DraftPurchaseRequests =
                        draftPurchaseRequests,

                    SubmittedPurchaseRequests =
                        submittedPurchaseRequests,

                    DraftPurchaseOrders =
                        draftPurchaseOrders,

                    ConfirmedPurchaseOrders =
                        confirmedPurchaseOrders,

                    CompletedGoodsReceipts =
                        completedGoodsReceipts
                },

            LowStockProducts =
                lowStockProducts,

            RecentPurchaseRequests =
                recentPurchaseRequests,

            RecentPurchaseOrders =
                recentPurchaseOrders,

            RecentGoodsReceipts =
                recentGoodsReceipts
        };
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
}