using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Unit;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(
        IUnitService unitService)
    {
        _unitService = unitService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units =
            await _unitService.GetAllAsync();

        return Ok(units);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var unit =
            await _unitService.GetByIdAsync(id);

        return Ok(unit);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(
        CreateUnitDto request)
    {
        var unit =
            await _unitService.CreateAsync(request);

        return Ok(unit);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Update(
        int id,
        UpdateUnitDto request)
    {
        var unit =
            await _unitService.UpdateAsync(
                id,
                request);

        return Ok(unit);
    }
}