using SupplyFlow.Application.DTOs.Common;

namespace SupplyFlow.Application.DTOs.Reports;

public class LowStockReportQueryDto
    : PagedQueryDto
{
    public int? WarehouseId { get; set; }

    public int? ProductId { get; set; }

    public int? CategoryId { get; set; }
}