using SupplyFlow.Application.DTOs.Common;

namespace SupplyFlow.Application.DTOs.Reports;

public class InventoryTransactionReportResultDto
{
    public PagedResultDto<InventoryTransactionReportDto>
        Data
    { get; set; } = new();

    public InventoryTransactionReportSummaryDto
        Summary
    { get; set; } = new();
}