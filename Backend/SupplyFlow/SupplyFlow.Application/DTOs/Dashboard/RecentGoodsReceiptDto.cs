namespace SupplyFlow.Application.DTOs.Dashboard;

public class RecentGoodsReceiptDto
{
    public int Id { get; set; }

    public string GRNNumber { get; set; }
        = string.Empty;

    public string PurchaseOrderNumber { get; set; }
        = string.Empty;

    public string WarehouseName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public DateTime ReceiptDate { get; set; }
}