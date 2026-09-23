using SupplyFlow.Application.DTOs.User;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();

    Task<UserDto> GetByIdAsync(int id);

    Task<UserDto> CreateAsync(CreateUserDto request);

    Task<UserDto> UpdateAsync(
        int id,
        UpdateUserDto request);
}