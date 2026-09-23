using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Reports;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(
        IReportService reportService)
    {
        _reportService = reportService;
    }


    // =========================================================
    // INVENTORY STOCK REPORT
    // GET: api/Reports/inventory-stock
    // =========================================================

    [HttpGet("inventory-stock")]
    public async Task<IActionResult>
        GetInventoryStockReport(
            [FromQuery]
            InventoryStockReportQueryDto query)
    {
        var result =
            await _reportService
                .GetInventoryStockReportAsync(query);

        return Ok(result);
    }


    // =========================================================
    // INVENTORY TRANSACTION REPORT
    // GET: api/Reports/inventory-transactions
    // =========================================================

    [HttpGet("inventory-transactions")]
    public async Task<IActionResult>
        GetInventoryTransactionReport(
            [FromQuery]
            InventoryTransactionReportQueryDto query)
    {
        var result =
            await _reportService
                .GetInventoryTransactionReportAsync(
                    query);

        return Ok(result);
    }


    // =========================================================
    // LOW STOCK REPORT
    // GET: api/Reports/low-stock
    // =========================================================

    [HttpGet("low-stock")]
    public async Task<IActionResult>
        GetLowStockReport(
            [FromQuery]
            LowStockReportQueryDto query)
    {
        var result =
            await _reportService
                .GetLowStockReportAsync(query);

        return Ok(result);
    }


    // =========================================================
    // PURCHASE ORDER REPORT
    // GET: api/Reports/purchase-orders
    // =========================================================

    [HttpGet("purchase-orders")]
    public async Task<IActionResult>
        GetPurchaseOrderReport(
            [FromQuery]
            PurchaseOrderReportQueryDto query)
    {
        var result =
            await _reportService
                .GetPurchaseOrderReportAsync(query);

        return Ok(result);
    }
}