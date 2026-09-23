using SupplyFlow.Application.DTOs.Reports;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IReportService
{
    Task<InventoryStockReportResultDto>
        GetInventoryStockReportAsync(
            InventoryStockReportQueryDto query);

    Task<InventoryTransactionReportResultDto>
        GetInventoryTransactionReportAsync(
            InventoryTransactionReportQueryDto query);

    Task<LowStockReportResultDto>
        GetLowStockReportAsync(
            LowStockReportQueryDto query);

    Task<PurchaseOrderReportResultDto>
        GetPurchaseOrderReportAsync(
            PurchaseOrderReportQueryDto query);
}