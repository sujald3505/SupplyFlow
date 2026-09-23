namespace SupplyFlow.Application.DTOs.Dashboard;

public class RecentPurchaseRequestDto
{
    public int Id { get; set; }

    public string RequestNumber { get; set; }
        = string.Empty;

    public string RequestedBy { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public DateTime CreatedAt { get; set; }
}