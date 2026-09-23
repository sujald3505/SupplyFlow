namespace SupplyFlow.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public int UserId { get; set; }

    public int CompanyId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}