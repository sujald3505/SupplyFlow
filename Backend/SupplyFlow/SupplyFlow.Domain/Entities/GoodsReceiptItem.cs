using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class GoodsReceiptItem : BaseEntity
{
    public int GoodsReceiptId { get; set; }

    public int ProductId { get; set; }

    public int PurchaseOrderItemId { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal? AcceptedQuantity { get; set; }

    public decimal? RejectedQuantity { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public GoodsReceipt GoodsReceipt { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
}