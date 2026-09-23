namespace SupplyFlow.Application.DTOs.Dashboard;

public class DashboardDto
{
    public DashboardSummaryDto Summary { get; set; }
        = new();

    public List<LowStockDashboardDto> LowStockProducts { get; set; }
        = new();

    public List<RecentPurchaseRequestDto> RecentPurchaseRequests { get; set; }
        = new();

    public List<RecentPurchaseOrderDto> RecentPurchaseOrders { get; set; }
        = new();

    public List<RecentGoodsReceiptDto> RecentGoodsReceipts { get; set; }
        = new();
}