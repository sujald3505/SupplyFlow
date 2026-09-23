namespace SupplyFlow.Application.DTOs.Reports;

public class PurchaseOrderReportSummaryDto
{
    public int TotalPurchaseOrders { get; set; }

    public decimal TotalPurchaseAmount { get; set; }

    public int DraftCount { get; set; }

    public int SentCount { get; set; }

    public int ConfirmedCount { get; set; }

    public int PartiallyReceivedCount { get; set; }

    public int CompletedCount { get; set; }

    public int CancelledCount { get; set; }
}