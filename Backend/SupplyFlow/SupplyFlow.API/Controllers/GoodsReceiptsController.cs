using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.GoodsReceipt;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoodsReceiptsController : ControllerBase
{
    private readonly IGoodsReceiptService
        _goodsReceiptService;

    public GoodsReceiptsController(
        IGoodsReceiptService goodsReceiptService)
    {
        _goodsReceiptService =
            goodsReceiptService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var goodsReceipts =
            await _goodsReceiptService
                .GetAllAsync();

        return Ok(goodsReceipts);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var goodsReceipt =
            await _goodsReceiptService
                .GetByIdAsync(id);

        return Ok(goodsReceipt);
    }


    [HttpPost]
    [Authorize(Roles =
        "Admin,WarehouseManager,PurchaseManager")]
    public async Task<IActionResult> Create(
        CreateGoodsReceiptDto request)
    {
        var goodsReceipt =
            await _goodsReceiptService
                .CreateAsync(request);

        return Ok(goodsReceipt);
    }


    [HttpPost("{id:int}/complete")]
    [Authorize(Roles =
        "Admin,WarehouseManager")]
    public async Task<IActionResult> Complete(
        int id)
    {
        var goodsReceipt =
            await _goodsReceiptService
                .CompleteAsync(id);

        return Ok(goodsReceipt);
    }


    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles =
        "Admin,WarehouseManager")]
    public async Task<IActionResult> Cancel(
        int id)
    {
        var goodsReceipt =
            await _goodsReceiptService
                .CancelAsync(id);

        return Ok(goodsReceipt);
    }
}