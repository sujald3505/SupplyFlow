using SupplyFlow.Application.DTOs.Role;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllAsync();

    Task<RoleDto> CreateAsync(CreateRoleDto request);
}