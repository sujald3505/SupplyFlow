namespace SupplyFlow.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public int CompanyId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Phone { get; set; }
}