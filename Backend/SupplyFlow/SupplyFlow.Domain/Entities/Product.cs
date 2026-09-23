//using SupplyFlow.Domain.Common;

//namespace SupplyFlow.Domain.Entities;

//public class Product : BaseEntity
//{
//    public int CompanyId { get; set; }

//    public int CategoryId { get; set; }

//    public int UnitId { get; set; }

//    public string Name { get; set; } = string.Empty;

//    public string SKU { get; set; } = string.Empty;

//    public string? Description { get; set; }

//    public decimal CostPrice { get; set; }

//    public decimal SellingPrice { get; set; }

//    public decimal MinimumStockLevel { get; set; }

//    public bool IsActive { get; set; } = true;

//    // Navigation Properties
//    public Company Company { get; set; } = null!;

//    public Category Category { get; set; } = null!;

//    public Unit Unit { get; set; } = null!;

//    public ICollection<WarehouseStock> WarehouseStocks { get; set; }
//    = new List<WarehouseStock>();

//    public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
//        = new List<InventoryTransaction>();

//    public ICollection<PurchaseRequestItem> PurchaseRequestItems { get; set; }
//    = new List<PurchaseRequestItem>();

//    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
//        = new List<PurchaseOrderItem>();

//    public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; }
//    = new List<GoodsReceiptItem>();
//}

using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Product : BaseEntity
{
    public int CompanyId { get; set; }

    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MinimumStockLevel { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public Unit Unit { get; set; } = null!;

    public ICollection<WarehouseStock> WarehouseStocks { get; set; }
    = new List<WarehouseStock>();

    public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        = new List<InventoryTransaction>();

    public ICollection<PurchaseRequestItem> PurchaseRequestItems { get; set; }
    = new List<PurchaseRequestItem>();

    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        = new List<PurchaseOrderItem>();

    public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    = new List<GoodsReceiptItem>();
}