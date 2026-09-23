using SupplyFlow.Application.DTOs.Common;
using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Application.DTOs.Reports;

public class PurchaseOrderReportQueryDto
    : PagedQueryDto
{
    public int? SupplierId { get; set; }

    public int? WarehouseId { get; set; }

    public PurchaseOrderStatus? Status { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}