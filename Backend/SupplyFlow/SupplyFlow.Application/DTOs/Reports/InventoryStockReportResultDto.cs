using SupplyFlow.Application.DTOs.Common;

namespace SupplyFlow.Application.DTOs.Reports;

public class InventoryStockReportResultDto
{
    public PagedResultDto<InventoryStockReportDto>
        Data
    { get; set; } = new();

    public InventoryStockReportSummaryDto
        Summary
    { get; set; } = new();
}