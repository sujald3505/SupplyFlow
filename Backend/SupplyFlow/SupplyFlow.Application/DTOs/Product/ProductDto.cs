//namespace SupplyFlow.Application.DTOs.Product;

//public class ProductDto
//{
//    public int Id { get; set; }

//    public int CategoryId { get; set; }

//    public string CategoryName { get; set; } = string.Empty;

//    public int UnitId { get; set; }

//    public string UnitName { get; set; } = string.Empty;

//    public string UnitSymbol { get; set; } = string.Empty;

//    public string Name { get; set; } = string.Empty;

//    public string SKU { get; set; } = string.Empty;

//    public string? Description { get; set; }

//    public decimal CostPrice { get; set; }

//    public decimal SellingPrice { get; set; }

//    public decimal MinimumStockLevel { get; set; }

//    public bool IsActive { get; set; }
//}

namespace SupplyFlow.Application.DTOs.Product;

public class ProductDto
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int UnitId { get; set; }

    public string UnitName { get; set; } = string.Empty;

    public string UnitSymbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal MinimumStockLevel { get; set; }

    public bool IsActive { get; set; }
}