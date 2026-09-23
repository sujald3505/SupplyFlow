using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.User;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Entities;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UserService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<UserDto>> GetAllAsync()
    {
        var companyId =
            _currentUser.CompanyId;

        if (!companyId.HasValue)
            throw new UnauthorizedAccessException();

        return await _context.Users
            .Include(x => x.Role)
            .Where(x =>
                x.CompanyId == companyId.Value)
            .OrderBy(x => x.FullName)
            .Select(x => new UserDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Phone = x.Phone,
                RoleId = x.RoleId,
                RoleName = x.Role.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    public async Task<UserDto> GetByIdAsync(
        int id)
    {
        var companyId =
            _currentUser.CompanyId;

        if (!companyId.HasValue)
            throw new UnauthorizedAccessException();

        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId.Value);

        if (user == null)
            throw new KeyNotFoundException(
                "User not found.");

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            RoleId = user.RoleId,
            RoleName = user.Role.Name,
            IsActive = user.IsActive
        };
    }


    public async Task<UserDto> CreateAsync(
        CreateUserDto request)
    {
        var companyId =
            _currentUser.CompanyId;

        if (!companyId.HasValue)
            throw new UnauthorizedAccessException();

        var emailExists = await _context.Users
            .AnyAsync(x =>
                x.CompanyId == companyId.Value &&
                x.Email == request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "Email already exists.");
        }

        var roleExists = await _context.Roles
            .AnyAsync(x =>
                x.Id == request.RoleId &&
                x.IsActive);

        if (!roleExists)
        {
            throw new KeyNotFoundException(
                "Role not found.");
        }

        var user = new Domain.Entities.User
        {
            CompanyId = companyId.Value,
            RoleId = request.RoleId,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.Password),
            Phone = request.Phone?.Trim(),
            IsActive = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var role = await _context.Roles
            .FirstAsync(x => x.Id == user.RoleId);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            RoleId = user.RoleId,
            RoleName = role.Name,
            IsActive = user.IsActive
        };
    }


    public async Task<UserDto> UpdateAsync(
        int id,
        UpdateUserDto request)
    {
        var companyId =
            _currentUser.CompanyId;

        if (!companyId.HasValue)
            throw new UnauthorizedAccessException();

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId.Value);

        if (user == null)
            throw new KeyNotFoundException(
                "User not found.");

        var roleExists = await _context.Roles
            .AnyAsync(x =>
                x.Id == request.RoleId &&
                x.IsActive);

        if (!roleExists)
        {
            throw new KeyNotFoundException(
                "Role not found.");
        }

        user.FullName =
            request.FullName.Trim();

        user.Phone =
            request.Phone?.Trim();

        user.RoleId =
            request.RoleId;

        user.IsActive =
            request.IsActive;

        await _context.SaveChangesAsync();

        var role = await _context.Roles
            .FirstAsync(x =>
                x.Id == user.RoleId);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            RoleId = user.RoleId,
            RoleName = role.Name,
            IsActive = user.IsActive
        };
    }
}