namespace SupplyFlow.Application.DTOs.Product;

public class UpdateProductDto
{
    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MinimumStockLevel { get; set; }

    public bool IsActive { get; set; }
}