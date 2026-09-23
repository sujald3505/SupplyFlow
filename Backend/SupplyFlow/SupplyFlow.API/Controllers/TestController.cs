using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public TestController(
        ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            message = "Protected API accessed successfully",

            userId = _currentUser.UserId,

            companyId = _currentUser.CompanyId,

            email = _currentUser.Email,

            role = _currentUser.Role
        });
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            message =
                "Only Admin can access this endpoint."
        });
    }
}