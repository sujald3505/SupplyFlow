using SupplyFlow.Domain.Common;
using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Domain.Entities;

public class GoodsReceipt : BaseEntity
{
    public int CompanyId { get; set; }

    public string GRNNumber { get; set; } = string.Empty;

    public int PurchaseOrderId { get; set; }

    public int WarehouseId { get; set; }

    public int ReceivedByUserId { get; set; }

    public GoodsReceiptStatus Status { get; set; }
        = GoodsReceiptStatus.Draft;

    public DateTime ReceiptDate { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public User ReceivedByUser { get; set; } = null!;

    public ICollection<GoodsReceiptItem> Items { get; set; }
        = new List<GoodsReceiptItem>();
}