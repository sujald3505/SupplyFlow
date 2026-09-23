namespace SupplyFlow.Application.DTOs.GoodsReceipt;

public class GoodsReceiptDto
{
    public int Id { get; set; }

    public string GRNNumber { get; set; }
        = string.Empty;

    public int PurchaseOrderId { get; set; }

    public string PurchaseOrderNumber { get; set; }
        = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public int ReceivedByUserId { get; set; }

    public string ReceivedByUserName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public DateTime ReceiptDate { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<GoodsReceiptItemDto> Items { get; set; }
        = new();
}