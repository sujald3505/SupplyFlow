namespace SupplyFlow.Application.DTOs.GoodsReceipt;

public class GoodsReceiptItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public int PurchaseOrderItemId { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal? AcceptedQuantity { get; set; }

    public decimal? RejectedQuantity { get; set; }

    public string? Remarks { get; set; }
}