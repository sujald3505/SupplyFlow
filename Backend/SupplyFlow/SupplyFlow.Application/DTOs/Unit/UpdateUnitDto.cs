namespace SupplyFlow.Application.DTOs.Unit;

public class UpdateUnitDto
{
    public string Name { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}