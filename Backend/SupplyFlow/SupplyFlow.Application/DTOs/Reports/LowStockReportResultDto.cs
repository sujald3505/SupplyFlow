using SupplyFlow.Application.DTOs.Common;

namespace SupplyFlow.Application.DTOs.Reports;

public class LowStockReportResultDto
{
    public PagedResultDto<LowStockReportDto>
        Data
    { get; set; } = new();

    public LowStockReportSummaryDto
        Summary
    { get; set; } = new();
}