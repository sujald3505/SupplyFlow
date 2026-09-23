namespace SupplyFlow.Application.DTOs.Reports;

public class InventoryStockReportSummaryDto
{
    public int TotalRecords { get; set; }

    public decimal TotalQuantity { get; set; }

    public decimal TotalInventoryValue { get; set; }

    public int LowStockCount { get; set; }

    public int OutOfStockCount { get; set; }
}