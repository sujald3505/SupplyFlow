namespace SupplyFlow.Application.DTOs.Unit;

public class UnitDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}