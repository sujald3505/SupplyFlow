using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.PurchaseRequest;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseRequestsController : ControllerBase
{
    private readonly IPurchaseRequestService
        _purchaseRequestService;

    public PurchaseRequestsController(
        IPurchaseRequestService purchaseRequestService)
    {
        _purchaseRequestService =
            purchaseRequestService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var requests =
            await _purchaseRequestService
                .GetAllAsync();

        return Ok(requests);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var request =
            await _purchaseRequestService
                .GetByIdAsync(id);

        return Ok(request);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePurchaseRequestDto request)
    {
        var purchaseRequest =
            await _purchaseRequestService
                .CreateAsync(request);

        return Ok(purchaseRequest);
    }


    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> Submit(
        int id)
    {
        var purchaseRequest =
            await _purchaseRequestService
                .SubmitAsync(id);

        return Ok(purchaseRequest);
    }


    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Admin,PurchaseManager")]
    public async Task<IActionResult> Approve(
        int id)
    {
        var purchaseRequest =
            await _purchaseRequestService
                .ApproveAsync(id);

        return Ok(purchaseRequest);
    }


    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = "Admin,PurchaseManager")]
    public async Task<IActionResult> Reject(
        int id,
        RejectPurchaseRequestDto request)
    {
        var purchaseRequest =
            await _purchaseRequestService
                .RejectAsync(id, request);

        return Ok(purchaseRequest);
    }
}