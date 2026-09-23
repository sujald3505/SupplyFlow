using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.User;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users =
            await _userService.GetAllAsync();

        return Ok(users);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user =
            await _userService.GetByIdAsync(id);

        return Ok(user);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateUserDto request)
    {
        var user =
            await _userService.CreateAsync(request);

        return Ok(user);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateUserDto request)
    {
        var user =
            await _userService.UpdateAsync(
                id,
                request);

        return Ok(user);
    }
}