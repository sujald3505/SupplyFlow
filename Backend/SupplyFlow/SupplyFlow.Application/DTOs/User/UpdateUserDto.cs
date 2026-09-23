namespace SupplyFlow.Application.DTOs.User;

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }
}