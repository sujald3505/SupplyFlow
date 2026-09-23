namespace SupplyFlow.Application.DTOs.Dashboard;

public class RecentPurchaseOrderDto
{
    public int Id { get; set; }

    public string PurchaseOrderNumber { get; set; }
        = string.Empty;

    public string SupplierName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}