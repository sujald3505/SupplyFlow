using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Company;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(
        ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentCompany()
    {
        var company =
            await _companyService
                .GetCurrentCompanyAsync();

        return Ok(company);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateCompanyDto request)
    {
        var company =
            await _companyService
                .UpdateAsync(request);

        return Ok(company);
    }
}