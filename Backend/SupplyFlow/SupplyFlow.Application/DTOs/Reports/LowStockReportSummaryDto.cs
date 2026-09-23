namespace SupplyFlow.Application.DTOs.Reports;

public class LowStockReportSummaryDto
{
    public int TotalLowStockProducts { get; set; }

    public decimal TotalShortageQuantity { get; set; }

    public int OutOfStockProducts { get; set; }
}