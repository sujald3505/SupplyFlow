namespace SupplyFlow.Application.DTOs.Reports;

public class InventoryTransactionReportSummaryDto
{
    public int TotalTransactions { get; set; }

    public decimal TotalTransactionQuantity { get; set; }

    public int StockInTransactions { get; set; }

    public int StockOutTransactions { get; set; }
}