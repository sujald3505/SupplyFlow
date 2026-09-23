using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Role;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.Role;

public class RoleService : IRoleService
{
    private readonly SupplyFlowDbContext _context;

    public RoleService(
        SupplyFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        return await _context.Roles
            .OrderBy(x => x.Name)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<RoleDto> CreateAsync(
        CreateRoleDto request)
    {
        var roleExists = await _context.Roles
            .AnyAsync(x =>
                x.Name == request.Name.Trim());

        if (roleExists)
        {
            throw new InvalidOperationException(
                "Role already exists.");
        }

        var role = new SupplyFlow.Domain.Entities.Role
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            IsActive = true
        };

        _context.Roles.Add(role);

        await _context.SaveChangesAsync();

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive
        };
    }
}