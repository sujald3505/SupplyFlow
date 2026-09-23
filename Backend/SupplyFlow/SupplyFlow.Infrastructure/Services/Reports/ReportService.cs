using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Common;
using SupplyFlow.Application.DTOs.Reports;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.Reports;

public class ReportService : IReportService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ReportService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    // =========================================================
    // INVENTORY STOCK REPORT
    // =========================================================

    public async Task<InventoryStockReportResultDto>
        GetInventoryStockReportAsync(
            InventoryStockReportQueryDto query)
    {
        var companyId = GetCompanyId();

        var stockQuery =
            _context.WarehouseStocks
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    !x.Product.IsDeleted &&
                    !x.Warehouse.IsDeleted &&
                    !x.Product.Category.IsDeleted &&
                    !x.Product.Unit.IsDeleted);


        // ============================
        // FILTERS
        // ============================

        if (query.WarehouseId.HasValue)
        {
            stockQuery = stockQuery.Where(x =>
                x.WarehouseId == query.WarehouseId.Value);
        }

        if (query.ProductId.HasValue)
        {
            stockQuery = stockQuery.Where(x =>
                x.ProductId == query.ProductId.Value);
        }

        if (query.CategoryId.HasValue)
        {
            stockQuery = stockQuery.Where(x =>
                x.Product.CategoryId ==
                query.CategoryId.Value);
        }


        // ============================
        // SEARCH
        // ============================

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            stockQuery = stockQuery.Where(x =>
                x.Product.Name.ToLower().Contains(search) ||
                x.Product.SKU.ToLower().Contains(search) ||
                x.Warehouse.Name.ToLower().Contains(search));
        }


        // ============================
        // SUMMARY
        // ============================

        var totalRecords =
            await stockQuery.CountAsync();

        var totalQuantity =
            await stockQuery.SumAsync(x =>
                (decimal?)x.Quantity) ?? 0;

        var totalInventoryValue =
            await stockQuery.SumAsync(x =>
                (decimal?)(x.Quantity *
                           x.Product.CostPrice)) ?? 0;

        var lowStockCount =
            await stockQuery.CountAsync(x =>
                x.Quantity <
                x.Product.MinimumStockLevel);

        var outOfStockCount =
            await stockQuery.CountAsync(x =>
                x.Quantity <= 0);


        // ============================
        // SORTING
        // ============================

        var sortBy =
            query.SortBy?
                .Trim()
                .ToLower();

        stockQuery =
            (sortBy, query.SortDescending) switch
            {
                ("productname", false) =>
                    stockQuery.OrderBy(x =>
                        x.Product.Name),

                ("productname", true) =>
                    stockQuery.OrderByDescending(x =>
                        x.Product.Name),

                ("sku", false) =>
                    stockQuery.OrderBy(x =>
                        x.Product.SKU),

                ("sku", true) =>
                    stockQuery.OrderByDescending(x =>
                        x.Product.SKU),

                ("warehouse", false) =>
                    stockQuery.OrderBy(x =>
                        x.Warehouse.Name),

                ("warehouse", true) =>
                    stockQuery.OrderByDescending(x =>
                        x.Warehouse.Name),

                ("quantity", false) =>
                    stockQuery.OrderBy(x =>
                        x.Quantity),

                ("quantity", true) =>
                    stockQuery.OrderByDescending(x =>
                        x.Quantity),

                ("inventoryvalue", false) =>
                    stockQuery.OrderBy(x =>
                        x.Quantity *
                        x.Product.CostPrice),

                ("inventoryvalue", true) =>
                    stockQuery.OrderByDescending(x =>
                        x.Quantity *
                        x.Product.CostPrice),

                _ =>
                    stockQuery.OrderByDescending(x =>
                        x.CreatedAt)
            };


        // ============================
        // PAGINATION
        // ============================

        var items =
            await stockQuery
                .Skip(
                    (query.PageNumber - 1) *
                    query.PageSize)
                .Take(query.PageSize)
                .Select(x =>
                    new InventoryStockReportDto
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        SKU = x.Product.SKU,

                        CategoryName =
                            x.Product.Category.Name,

                        UnitName =
                            x.Product.Unit.Name,

                        WarehouseId =
                            x.WarehouseId,

                        WarehouseName =
                            x.Warehouse.Name,

                        CurrentQuantity =
                            x.Quantity,

                        MinimumStockLevel =
                            x.Product.MinimumStockLevel,

                        StockStatus =
                            x.Quantity <= 0
                                ? "Out of Stock"
                                : x.Quantity <
                                  x.Product.MinimumStockLevel
                                    ? "Low Stock"
                                    : "In Stock",

                        CostPrice =
                            x.Product.CostPrice,

                        InventoryValue =
                            x.Quantity *
                            x.Product.CostPrice
                    })
                .ToListAsync();


        return new InventoryStockReportResultDto
        {
            Data =
                new PagedResultDto<
                    InventoryStockReportDto>
                {
                    Items = items,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalCount = totalRecords
                },

            Summary =
                new InventoryStockReportSummaryDto
                {
                    TotalRecords = totalRecords,
                    TotalQuantity = totalQuantity,
                    TotalInventoryValue =
                        totalInventoryValue,
                    LowStockCount = lowStockCount,
                    OutOfStockCount =
                        outOfStockCount
                }
        };
    }


    // =========================================================
    // INVENTORY TRANSACTION REPORT
    // =========================================================

    public async Task<InventoryTransactionReportResultDto>
        GetInventoryTransactionReportAsync(
            InventoryTransactionReportQueryDto query)
    {
        var companyId = GetCompanyId();

        var transactionQuery =
            _context.InventoryTransactions
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    !x.Product.IsDeleted &&
                    !x.Warehouse.IsDeleted);


        // ============================
        // FILTERS
        // ============================

        if (query.WarehouseId.HasValue)
        {
            transactionQuery =
                transactionQuery.Where(x =>
                    x.WarehouseId ==
                    query.WarehouseId.Value);
        }

        if (query.ProductId.HasValue)
        {
            transactionQuery =
                transactionQuery.Where(x =>
                    x.ProductId ==
                    query.ProductId.Value);
        }

        if (query.TransactionType.HasValue)
        {
            transactionQuery =
                transactionQuery.Where(x =>
                    x.TransactionType ==
                    query.TransactionType.Value);
        }


        // ============================
        // DATE RANGE
        // ============================

        if (query.FromDate.HasValue)
        {
            var fromDate =
                query.FromDate.Value.Date;

            transactionQuery =
                transactionQuery.Where(x =>
                    x.CreatedAt >= fromDate);
        }

        if (query.ToDate.HasValue)
        {
            var toDate =
                query.ToDate.Value.Date
                    .AddDays(1);

            transactionQuery =
                transactionQuery.Where(x =>
                    x.CreatedAt < toDate);
        }


        // ============================
        // SEARCH
        // ============================

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search =
                query.Search.Trim().ToLower();

            transactionQuery =
                transactionQuery.Where(x =>
                    x.Product.Name
                        .ToLower()
                        .Contains(search) ||

                    x.Product.SKU
                        .ToLower()
                        .Contains(search) ||

                    x.Warehouse.Name
                        .ToLower()
                        .Contains(search) ||

                    (x.ReferenceNumber != null &&
                     x.ReferenceNumber
                        .ToLower()
                        .Contains(search)));
        }


        // ============================
        // SUMMARY
        // ============================

        var totalTransactions =
            await transactionQuery.CountAsync();

        var totalTransactionQuantity =
            await transactionQuery.SumAsync(x =>
                (decimal?)x.Quantity) ?? 0;


        // Change StockIn / StockOut only if your
        // InventoryTransactionType enum uses
        // different names.

        var stockInTransactions =
            await transactionQuery.CountAsync(x =>
                x.TransactionType ==
                InventoryTransactionType.StockIn);

        var stockOutTransactions =
            await transactionQuery.CountAsync(x =>
                x.TransactionType ==
                InventoryTransactionType.StockOut);


        // ============================
        // SORTING
        // ============================

        var sortBy =
            query.SortBy?
                .Trim()
                .ToLower();

        transactionQuery =
            (sortBy, query.SortDescending) switch
            {
                ("date", false) =>
                    transactionQuery.OrderBy(x =>
                        x.CreatedAt),

                ("date", true) =>
                    transactionQuery.OrderByDescending(x =>
                        x.CreatedAt),

                ("productname", false) =>
                    transactionQuery.OrderBy(x =>
                        x.Product.Name),

                ("productname", true) =>
                    transactionQuery.OrderByDescending(x =>
                        x.Product.Name),

                ("warehouse", false) =>
                    transactionQuery.OrderBy(x =>
                        x.Warehouse.Name),

                ("warehouse", true) =>
                    transactionQuery.OrderByDescending(x =>
                        x.Warehouse.Name),

                ("quantity", false) =>
                    transactionQuery.OrderBy(x =>
                        x.Quantity),

                ("quantity", true) =>
                    transactionQuery.OrderByDescending(x =>
                        x.Quantity),

                ("transactiontype", false) =>
                    transactionQuery.OrderBy(x =>
                        x.TransactionType),

                ("transactiontype", true) =>
                    transactionQuery.OrderByDescending(x =>
                        x.TransactionType),

                _ =>
                    transactionQuery.OrderByDescending(x =>
                        x.CreatedAt)
            };


        // ============================
        // PAGINATION + DATA
        // ============================

        var items =
            await transactionQuery
                .Skip(
                    (query.PageNumber - 1) *
                    query.PageSize)
                .Take(query.PageSize)
                .Select(x =>
                    new InventoryTransactionReportDto
                    {
                        Id = x.Id,

                        CreatedAt =
                            x.CreatedAt,

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

                        TransactionType =
                            x.TransactionType
                                .ToString(),

                        Quantity =
                            x.Quantity,

                        PreviousQuantity =
                            x.PreviousQuantity,

                        NewQuantity =
                            x.NewQuantity,

                        ReferenceNumber =
                            x.ReferenceNumber,

                        Remarks =
                            x.Remarks
                    })
                .ToListAsync();


        return new InventoryTransactionReportResultDto
        {
            Data =
                new PagedResultDto<
                    InventoryTransactionReportDto>
                {
                    Items = items,
                    PageNumber =
                        query.PageNumber,
                    PageSize =
                        query.PageSize,
                    TotalCount =
                        totalTransactions
                },

            Summary =
                new InventoryTransactionReportSummaryDto
                {
                    TotalTransactions =
                        totalTransactions,

                    TotalTransactionQuantity =
                        totalTransactionQuantity,

                    StockInTransactions =
                        stockInTransactions,

                    StockOutTransactions =
                        stockOutTransactions
                }
        };
    }


    // =========================================================
    // LOW STOCK REPORT
    // Will implement in STEP 26.9
    // =========================================================

    public async Task<LowStockReportResultDto>
    GetLowStockReportAsync(
        LowStockReportQueryDto query)
    {
        var companyId = GetCompanyId();

        // =====================================
        // BASE QUERY
        // =====================================

        var lowStockQuery =
            _context.WarehouseStocks
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    !x.Product.IsDeleted &&
                    !x.Warehouse.IsDeleted &&
                    !x.Product.Category.IsDeleted &&
                    x.Quantity <
                    x.Product.MinimumStockLevel);


        // =====================================
        // FILTERS
        // =====================================

        if (query.WarehouseId.HasValue)
        {
            lowStockQuery =
                lowStockQuery.Where(x =>
                    x.WarehouseId ==
                    query.WarehouseId.Value);
        }

        if (query.ProductId.HasValue)
        {
            lowStockQuery =
                lowStockQuery.Where(x =>
                    x.ProductId ==
                    query.ProductId.Value);
        }

        if (query.CategoryId.HasValue)
        {
            lowStockQuery =
                lowStockQuery.Where(x =>
                    x.Product.CategoryId ==
                    query.CategoryId.Value);
        }


        // =====================================
        // SEARCH
        // =====================================

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search =
                query.Search.Trim().ToLower();

            lowStockQuery =
                lowStockQuery.Where(x =>
                    x.Product.Name
                        .ToLower()
                        .Contains(search) ||

                    x.Product.SKU
                        .ToLower()
                        .Contains(search) ||

                    x.Product.Category.Name
                        .ToLower()
                        .Contains(search) ||

                    x.Warehouse.Name
                        .ToLower()
                        .Contains(search));
        }


        // =====================================
        // SUMMARY
        // =====================================

        var totalLowStockProducts =
            await lowStockQuery.CountAsync();

        var totalShortageQuantity =
            await lowStockQuery.SumAsync(x =>
                (decimal?)(
                    x.Product.MinimumStockLevel -
                    x.Quantity)) ?? 0;

        var outOfStockProducts =
            await lowStockQuery.CountAsync(x =>
                x.Quantity <= 0);


        // =====================================
        // SORTING
        // =====================================

        var sortBy =
            query.SortBy?
                .Trim()
                .ToLower();

        lowStockQuery =
            (sortBy, query.SortDescending) switch
            {
                ("productname", false) =>
                    lowStockQuery
                        .OrderBy(x =>
                            x.Product.Name),

                ("productname", true) =>
                    lowStockQuery
                        .OrderByDescending(x =>
                            x.Product.Name),

                ("warehouse", false) =>
                    lowStockQuery
                        .OrderBy(x =>
                            x.Warehouse.Name),

                ("warehouse", true) =>
                    lowStockQuery
                        .OrderByDescending(x =>
                            x.Warehouse.Name),

                ("quantity", false) =>
                    lowStockQuery
                        .OrderBy(x =>
                            x.Quantity),

                ("quantity", true) =>
                    lowStockQuery
                        .OrderByDescending(x =>
                            x.Quantity),

                ("shortage", false) =>
                    lowStockQuery
                        .OrderBy(x =>
                            x.Product.MinimumStockLevel -
                            x.Quantity),

                ("shortage", true) =>
                    lowStockQuery
                        .OrderByDescending(x =>
                            x.Product.MinimumStockLevel -
                            x.Quantity),

                _ =>
                    lowStockQuery
                        .OrderBy(x =>
                            x.Quantity)
            };


        // =====================================
        // PAGINATION + DATA
        // =====================================

        var items =
            await lowStockQuery
                .Skip(
                    (query.PageNumber - 1) *
                    query.PageSize)
                .Take(query.PageSize)
                .Select(x =>
                    new LowStockReportDto
                    {
                        ProductId =
                            x.ProductId,

                        ProductName =
                            x.Product.Name,

                        SKU =
                            x.Product.SKU,

                        CategoryName =
                            x.Product.Category.Name,

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
                .ToListAsync();


        // =====================================
        // RESPONSE
        // =====================================

        return new LowStockReportResultDto
        {
            Data =
                new PagedResultDto<
                    LowStockReportDto>
                {
                    Items = items,

                    PageNumber =
                        query.PageNumber,

                    PageSize =
                        query.PageSize,

                    TotalCount =
                        totalLowStockProducts
                },

            Summary =
                new LowStockReportSummaryDto
                {
                    TotalLowStockProducts =
                        totalLowStockProducts,

                    TotalShortageQuantity =
                        totalShortageQuantity,

                    OutOfStockProducts =
                        outOfStockProducts
                }
        };
    }

    // =========================================================
    // PURCHASE ORDER REPORT
    // Will implement later
    // =========================================================

    public async Task<PurchaseOrderReportResultDto>
    GetPurchaseOrderReportAsync(
        PurchaseOrderReportQueryDto query)
    {
        var companyId = GetCompanyId();

        // =====================================
        // BASE QUERY
        // =====================================

        var purchaseOrderQuery =
            _context.PurchaseOrders
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    !x.IsDeleted &&
                    !x.Supplier.IsDeleted &&
                    !x.Warehouse.IsDeleted);


        // =====================================
        // FILTERS
        // =====================================

        if (query.SupplierId.HasValue)
        {
            purchaseOrderQuery =
                purchaseOrderQuery.Where(x =>
                    x.SupplierId ==
                    query.SupplierId.Value);
        }

        if (query.WarehouseId.HasValue)
        {
            purchaseOrderQuery =
                purchaseOrderQuery.Where(x =>
                    x.WarehouseId ==
                    query.WarehouseId.Value);
        }

        if (query.Status.HasValue)
        {
            purchaseOrderQuery =
                purchaseOrderQuery.Where(x =>
                    x.Status ==
                    query.Status.Value);
        }


        // =====================================
        // DATE RANGE
        // =====================================

        if (query.FromDate.HasValue)
        {
            var fromDate =
                query.FromDate.Value.Date;

            purchaseOrderQuery =
                purchaseOrderQuery.Where(x =>
                    x.OrderDate >= fromDate);
        }

        if (query.ToDate.HasValue)
        {
            var toDate =
                query.ToDate.Value.Date
                    .AddDays(1);

            purchaseOrderQuery =
                purchaseOrderQuery.Where(x =>
                    x.OrderDate < toDate);
        }


        // =====================================
        // SEARCH
        // =====================================

        if (!string.IsNullOrWhiteSpace(
                query.Search))
        {
            var search =
                query.Search.Trim().ToLower();

            purchaseOrderQuery =
                purchaseOrderQuery.Where(x =>
                    x.PurchaseOrderNumber
                        .ToLower()
                        .Contains(search) ||

                    x.Supplier.Name
                        .ToLower()
                        .Contains(search) ||

                    x.Warehouse.Name
                        .ToLower()
                        .Contains(search));
        }


        // =====================================
        // SUMMARY
        // =====================================

        var totalPurchaseOrders =
            await purchaseOrderQuery.CountAsync();

        var totalPurchaseAmount =
            await purchaseOrderQuery.SumAsync(x =>
                (decimal?)x.TotalAmount) ?? 0;


        var draftCount =
            await purchaseOrderQuery.CountAsync(x =>
                x.Status ==
                PurchaseOrderStatus.Draft);

        var sentCount =
            await purchaseOrderQuery.CountAsync(x =>
                x.Status ==
                PurchaseOrderStatus.Sent);

        var confirmedCount =
            await purchaseOrderQuery.CountAsync(x =>
                x.Status ==
                PurchaseOrderStatus.Confirmed);

        var partiallyReceivedCount =
            await purchaseOrderQuery.CountAsync(x =>
                x.Status ==
                PurchaseOrderStatus.PartiallyReceived);

        var completedCount =
            await purchaseOrderQuery.CountAsync(x =>
                x.Status ==
                PurchaseOrderStatus.Completed);

        var cancelledCount =
            await purchaseOrderQuery.CountAsync(x =>
                x.Status ==
                PurchaseOrderStatus.Cancelled);


        // =====================================
        // SORTING
        // =====================================

        var sortBy =
            query.SortBy?
                .Trim()
                .ToLower();

        purchaseOrderQuery =
            (sortBy, query.SortDescending) switch
            {
                ("ordernumber", false) =>
                    purchaseOrderQuery
                        .OrderBy(x =>
                            x.PurchaseOrderNumber),

                ("ordernumber", true) =>
                    purchaseOrderQuery
                        .OrderByDescending(x =>
                            x.PurchaseOrderNumber),

                ("supplier", false) =>
                    purchaseOrderQuery
                        .OrderBy(x =>
                            x.Supplier.Name),

                ("supplier", true) =>
                    purchaseOrderQuery
                        .OrderByDescending(x =>
                            x.Supplier.Name),

                ("warehouse", false) =>
                    purchaseOrderQuery
                        .OrderBy(x =>
                            x.Warehouse.Name),

                ("warehouse", true) =>
                    purchaseOrderQuery
                        .OrderByDescending(x =>
                            x.Warehouse.Name),

                ("date", false) =>
                    purchaseOrderQuery
                        .OrderBy(x =>
                            x.OrderDate),

                ("date", true) =>
                    purchaseOrderQuery
                        .OrderByDescending(x =>
                            x.OrderDate),

                ("amount", false) =>
                    purchaseOrderQuery
                        .OrderBy(x =>
                            x.TotalAmount),

                ("amount", true) =>
                    purchaseOrderQuery
                        .OrderByDescending(x =>
                            x.TotalAmount),

                _ =>
                    purchaseOrderQuery
                        .OrderByDescending(x =>
                            x.OrderDate)
            };


        // =====================================
        // PAGINATION + DATA
        // =====================================

        var items =
            await purchaseOrderQuery
                .Skip(
                    (query.PageNumber - 1) *
                    query.PageSize)
                .Take(query.PageSize)
                .Select(x =>
                    new PurchaseOrderReportDto
                    {
                        Id = x.Id,

                        PurchaseOrderNumber =
                            x.PurchaseOrderNumber,

                        SupplierId =
                            x.SupplierId,

                        SupplierName =
                            x.Supplier.Name,

                        WarehouseId =
                            x.WarehouseId,

                        WarehouseName =
                            x.Warehouse.Name,

                        Status =
                            x.Status.ToString(),

                        OrderDate =
                            x.OrderDate,

                        ExpectedDeliveryDate =
                            x.ExpectedDeliveryDate,

                        TotalAmount =
                            x.TotalAmount,

                        Remarks =
                            x.Remarks
                    })
                .ToListAsync();


        // =====================================
        // RESPONSE
        // =====================================

        return new PurchaseOrderReportResultDto
        {
            Data =
                new PagedResultDto<
                    PurchaseOrderReportDto>
                {
                    Items = items,

                    PageNumber =
                        query.PageNumber,

                    PageSize =
                        query.PageSize,

                    TotalCount =
                        totalPurchaseOrders
                },

            Summary =
                new PurchaseOrderReportSummaryDto
                {
                    TotalPurchaseOrders =
                        totalPurchaseOrders,

                    TotalPurchaseAmount =
                        totalPurchaseAmount,

                    DraftCount =
                        draftCount,

                    SentCount =
                        sentCount,

                    ConfirmedCount =
                        confirmedCount,

                    PartiallyReceivedCount =
                        partiallyReceivedCount,

                    CompletedCount =
                        completedCount,

                    CancelledCount =
                        cancelledCount
                }
        };
    }

    // =========================================================
    // CURRENT COMPANY
    // =========================================================

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