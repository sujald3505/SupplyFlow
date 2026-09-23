namespace SupplyFlow.Application.DTOs.Supplier;

public class UpdateSupplierDto
{
    public string Name { get; set; } = string.Empty;

    public string ContactPerson { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? GSTNumber { get; set; }

    public bool IsActive { get; set; }
}