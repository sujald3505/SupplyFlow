namespace SupplyFlow.Application.DTOs.Company;

public class UpdateCompanyDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? GSTNumber { get; set; }
}