using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupplyFlow.Application.DTOs.Auth;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// Explicit aliases to avoid namespace/type conflicts
using DomainUser = SupplyFlow.Domain.Entities.User;
using SecurityClaim = System.Security.Claims.Claim;

namespace SupplyFlow.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly SupplyFlowDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        SupplyFlowDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Email == request.Email &&
                x.IsActive);

        if (user == null)
            return null;

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user);

        var expiryMinutes = int.Parse(
            _configuration["JwtSettings:ExpiryMinutes"]!);

        return new LoginResponseDto
        {
            Token = token,

            Expiration = DateTime.UtcNow
                .AddMinutes(expiryMinutes),

            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name,
            UserId = user.Id,
            CompanyId = user.CompanyId
        };
    }

    public async Task<RegisterResponseDto?> RegisterAsync(
    RegisterRequestDto request)
    {
        // Check duplicate email
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (existingUser != null)
            return null;

        // Check company exists
        var companyExists = await _context.Companies
            .AnyAsync(x => x.Id == request.CompanyId);

        if (!companyExists)
            return null;

        // Check role exists
        var role = await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == request.RoleId);

        if (role == null)
            return null;

        // Create new user
        var user = new DomainUser
        {
            CompanyId = request.CompanyId,
            RoleId = request.RoleId,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password),
            Phone = request.Phone,
            IsActive = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserId = user.Id,
            CompanyId = user.CompanyId,
            RoleId = user.RoleId,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = role.Name,
            IsActive = user.IsActive
        };
    }
    private string GenerateJwtToken(
        DomainUser user)
    {
        var jwtSettings =
            _configuration.GetSection("JwtSettings");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                jwtSettings["Key"]!));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new SecurityClaim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new SecurityClaim(
                ClaimTypes.Email,
                user.Email),

            new SecurityClaim(
                ClaimTypes.Name,
                user.FullName),

            new SecurityClaim(
                ClaimTypes.Role,
                user.Role.Name),

            new SecurityClaim(
                "CompanyId",
                user.CompanyId.ToString())
        };

        var expiryMinutes = int.Parse(
            jwtSettings["ExpiryMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow
                .AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}