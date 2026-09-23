using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Sale;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(
        ISaleService saleService)
    {
        _saleService = saleService;
    }

    // GET: api/Sales
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales =
            await _saleService.GetAllAsync();

        return Ok(sales);
    }

    // GET: api/Sales/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var sale =
            await _saleService.GetByIdAsync(id);

        if (sale == null)
        {
            return NotFound(new
            {
                message = "Sale not found."
            });
        }

        return Ok(sale);
    }

    // POST: api/Sales
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateSaleDto request)
    {
        try
        {
            var sale =
                await _saleService.CreateAsync(request);

            return Ok(sale);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}