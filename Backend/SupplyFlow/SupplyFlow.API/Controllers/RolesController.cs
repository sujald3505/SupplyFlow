using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Role;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(
        IRoleService roleService)
    {
        _roleService = roleService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles =
            await _roleService.GetAllAsync();

        return Ok(roles);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRoleDto request)
    {
        var role =
            await _roleService.CreateAsync(request);

        return Ok(role);
    }
}