using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }

    public int ProductId { get; set; }

    public decimal OrderedQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    // Future GRN support
    public decimal ReceivedQuantity { get; set; }

    // Navigation Properties
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    = new List<GoodsReceiptItem>();
}