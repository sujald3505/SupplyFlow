using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Auth;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto request)
    {
        var result =
            await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new
            {
                message =
                    "Invalid email or password."
            });
        }

        return Ok(result);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(
    RegisterRequestDto request)
    {
        var result =
            await _authService.RegisterAsync(request);

        if (result == null)
        {
            return BadRequest(new
            {
                message =
                    "Registration failed. Email may already exist, or Company/Role is invalid."
            });
        }

        return Ok(result);
    }
}