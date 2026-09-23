namespace SupplyFlow.Application.DTOs.GoodsReceipt;

public class CreateGoodsReceiptDto
{
    public int PurchaseOrderId { get; set; }

    public DateTime? ReceiptDate { get; set; }

    public string? Remarks { get; set; }

    public List<CreateGoodsReceiptItemDto> Items { get; set; }
        = new();
}