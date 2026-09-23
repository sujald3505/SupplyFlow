namespace SupplyFlow.Application.DTOs.GoodsReceipt;

public class CreateGoodsReceiptItemDto
{
    public int ProductId { get; set; }

    public int PurchaseOrderItemId { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal? AcceptedQuantity { get; set; }

    public decimal? RejectedQuantity { get; set; }

    public string? Remarks { get; set; }
}