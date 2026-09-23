namespace SupplyFlow.Application.DTOs.WarehouseStock;

public class WarehouseStockDto
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string SKU { get; set; }
        = string.Empty;

    public string UnitName { get; set; }
        = string.Empty;

    public string UnitSymbol { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }
}