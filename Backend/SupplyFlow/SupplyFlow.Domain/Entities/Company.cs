using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? GSTNumber { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Unit> Units { get; set; } = new List<Unit>();

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

    public ICollection<WarehouseStock> WarehouseStocks { get; set; }
    = new List<WarehouseStock>();

    public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        = new List<InventoryTransaction>();

    public ICollection<PurchaseRequest> PurchaseRequests { get; set; }
    = new List<PurchaseRequest>();

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        = new List<PurchaseOrder>();

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; }
    = new List<GoodsReceipt>();
}