using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.SalesReturn;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesReturnsController : ControllerBase
{
    private readonly ISalesReturnService _salesReturnService;

    public SalesReturnsController(
        ISalesReturnService salesReturnService)
    {
        _salesReturnService = salesReturnService;
    }


    // =========================================================
    // GET ALL SALES RETURNS
    // GET: api/SalesReturns
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var returns =
            await _salesReturnService.GetAllAsync();

        return Ok(returns);
    }


    // =========================================================
    // GET SALES RETURN BY ID
    // GET: api/SalesReturns/1
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var salesReturn =
            await _salesReturnService
                .GetByIdAsync(id);

        if (salesReturn == null)
        {
            return NotFound(new
            {
                message = "Sales return not found."
            });
        }

        return Ok(salesReturn);
    }


    // =========================================================
    // CREATE SALES RETURN
    // POST: api/SalesReturns
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> Create(
    [FromBody] CreateSalesReturnDto request)
    {
        try
        {
            var salesReturn =
                await _salesReturnService
                    .CreateAsync(request);

            return Ok(salesReturn);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message,
                innerException = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
}