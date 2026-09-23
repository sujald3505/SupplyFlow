using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Warehouse : BaseEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Address { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public ICollection<WarehouseStock> WarehouseStocks { get; set; }
    = new List<WarehouseStock>();

    public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        = new List<InventoryTransaction>();

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    = new List<PurchaseOrder>();

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; }
    = new List<GoodsReceipt>();
}