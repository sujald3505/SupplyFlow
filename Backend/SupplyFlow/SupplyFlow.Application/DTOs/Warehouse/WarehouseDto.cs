namespace SupplyFlow.Application.DTOs.Warehouse;

public class WarehouseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Address { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public bool IsActive { get; set; }
}