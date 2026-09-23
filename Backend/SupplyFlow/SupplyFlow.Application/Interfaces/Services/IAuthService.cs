using SupplyFlow.Application.DTOs.Auth;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(
        LoginRequestDto request);

    Task<RegisterResponseDto?> RegisterAsync(
        RegisterRequestDto request);
}