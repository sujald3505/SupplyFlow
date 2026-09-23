using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Warehouse;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(
        IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var warehouses =
            await _warehouseService.GetAllAsync();

        return Ok(warehouses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var warehouse =
            await _warehouseService.GetByIdAsync(id);

        return Ok(warehouse);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(
        CreateWarehouseDto request)
    {
        var warehouse =
            await _warehouseService.CreateAsync(request);

        return Ok(warehouse);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Update(
        int id,
        UpdateWarehouseDto request)
    {
        var warehouse =
            await _warehouseService.UpdateAsync(
                id,
                request);

        return Ok(warehouse);
    }
}