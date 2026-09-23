using SupplyFlow.Domain.Common;
using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public int CompanyId { get; set; }

    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public int SupplierId { get; set; }

    public int WarehouseId { get; set; }

    public int? PurchaseRequestId { get; set; }

    public PurchaseOrderStatus Status { get; set; }
        = PurchaseOrderStatus.Draft;

    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public Supplier Supplier { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public PurchaseRequest? PurchaseRequest { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; }
        = new List<PurchaseOrderItem>();

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; }
    = new List<GoodsReceipt>();
}