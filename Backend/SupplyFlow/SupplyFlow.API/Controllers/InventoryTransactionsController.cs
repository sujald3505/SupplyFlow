using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.InventoryTransaction;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryTransactionsController
    : ControllerBase
{
    private readonly IInventoryTransactionService
        _transactionService;

    public InventoryTransactionsController(
        IInventoryTransactionService transactionService)
    {
        _transactionService =
            transactionService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions =
            await _transactionService
                .GetAllAsync();

        return Ok(transactions);
    }


    [HttpGet("warehouse/{warehouseId:int}")]
    public async Task<IActionResult> GetByWarehouse(
        int warehouseId)
    {
        var transactions =
            await _transactionService
                .GetByWarehouseIdAsync(
                    warehouseId);

        return Ok(transactions);
    }


    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(
        int productId)
    {
        var transactions =
            await _transactionService
                .GetByProductIdAsync(
                    productId);

        return Ok(transactions);
    }


    [HttpPost]
    [Authorize(Roles =
        "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(
        CreateInventoryTransactionDto request)
    {
        var transaction =
            await _transactionService
                .CreateAsync(request);

        return Ok(transaction);
    }
}