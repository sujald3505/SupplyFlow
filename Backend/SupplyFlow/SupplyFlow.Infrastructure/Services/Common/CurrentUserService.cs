using Microsoft.AspNetCore.Http;
using SupplyFlow.Application.Interfaces.Services;
using System.Security.Claims;

namespace SupplyFlow.Infrastructure.Services.Common;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor
            .HttpContext?
            .User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;


    public int? UserId =>
        int.TryParse(
            User?.FindFirst(
                ClaimTypes.NameIdentifier)?.Value,
            out var userId)
                ? userId
                : null;


    public int? CompanyId =>
        int.TryParse(
            User?.FindFirst(
                "CompanyId")?.Value,
            out var companyId)
                ? companyId
                : null;


    public string? Email =>
        User?.FindFirst(
            ClaimTypes.Email)?.Value;


    public string? Role =>
        User?.FindFirst(
            ClaimTypes.Role)?.Value;
}