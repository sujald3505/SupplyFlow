using SupplyFlow.Application.DTOs.Common;
using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Application.DTOs.Reports;

public class InventoryTransactionReportQueryDto
    : PagedQueryDto
{
    public int? WarehouseId { get; set; }

    public int? ProductId { get; set; }

    public InventoryTransactionType?
        TransactionType
    { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}