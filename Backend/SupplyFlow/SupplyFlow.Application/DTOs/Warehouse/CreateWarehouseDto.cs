namespace SupplyFlow.Application.DTOs.Warehouse;

public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public string? Address { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }
}