using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.PurchaseOrder;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService
        _purchaseOrderService;

    public PurchaseOrdersController(
        IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService =
            purchaseOrderService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var purchaseOrders =
            await _purchaseOrderService.GetAllAsync();

        return Ok(purchaseOrders);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var purchaseOrder =
            await _purchaseOrderService
                .GetByIdAsync(id);

        return Ok(purchaseOrder);
    }


    [HttpPost]
    [Authorize(Roles =
        "Admin,PurchaseManager")]
    public async Task<IActionResult> Create(
        CreatePurchaseOrderDto request)
    {
        var purchaseOrder =
            await _purchaseOrderService
                .CreateAsync(request);

        return Ok(purchaseOrder);
    }


    [HttpPost("{id:int}/send")]
    [Authorize(Roles =
        "Admin,PurchaseManager")]
    public async Task<IActionResult> Send(
        int id)
    {
        var purchaseOrder =
            await _purchaseOrderService
                .SendAsync(id);

        return Ok(purchaseOrder);
    }


    [HttpPost("{id:int}/confirm")]
    [Authorize(Roles =
        "Admin,PurchaseManager")]
    public async Task<IActionResult> Confirm(
        int id)
    {
        var purchaseOrder =
            await _purchaseOrderService
                .ConfirmAsync(id);

        return Ok(purchaseOrder);
    }


    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles =
        "Admin,PurchaseManager")]
    public async Task<IActionResult> Cancel(
        int id)
    {
        var purchaseOrder =
            await _purchaseOrderService
                .CancelAsync(id);

        return Ok(purchaseOrder);
    }
}