using SupplyFlow.Application.DTOs.Common;

namespace SupplyFlow.Application.DTOs.Reports;

public class PurchaseOrderReportResultDto
{
    public PagedResultDto<PurchaseOrderReportDto>
        Data
    { get; set; } = new();

    public PurchaseOrderReportSummaryDto
        Summary
    { get; set; } = new();
}