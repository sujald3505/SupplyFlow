using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseStocksController : ControllerBase
{
    private readonly IWarehouseStockService _stockService;

    public WarehouseStocksController(
        IWarehouseStockService stockService)
    {
        _stockService = stockService;
    }


    // GET: api/WarehouseStocks
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks =
            await _stockService.GetAllAsync();

        return Ok(stocks);
    }


    // GET: api/WarehouseStocks/warehouse/1
    [HttpGet("warehouse/{warehouseId:int}")]
    public async Task<IActionResult> GetByWarehouse(
        int warehouseId)
    {
        var stocks =
            await _stockService
                .GetByWarehouseIdAsync(
                    warehouseId);

        return Ok(stocks);
    }


    // GET: api/WarehouseStocks/product/1
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(
        int productId)
    {
        var stocks =
            await _stockService
                .GetByProductIdAsync(
                    productId);

        return Ok(stocks);
    }


    // GET: api/WarehouseStocks/low-stock
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var stocks =
            await _stockService
                .GetLowStockAsync();

        return Ok(stocks);
    }
}