using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Supplier;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(
        ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers =
            await _supplierService.GetAllAsync();

        return Ok(suppliers);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var supplier =
            await _supplierService.GetByIdAsync(id);

        return Ok(supplier);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,PurchaseManager")]
    public async Task<IActionResult> Create(
        CreateSupplierDto request)
    {
        var supplier =
            await _supplierService.CreateAsync(request);

        return Ok(supplier);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,PurchaseManager")]
    public async Task<IActionResult> Update(
        int id,
        UpdateSupplierDto request)
    {
        var supplier =
            await _supplierService.UpdateAsync(
                id,
                request);

        return Ok(supplier);
    }
}