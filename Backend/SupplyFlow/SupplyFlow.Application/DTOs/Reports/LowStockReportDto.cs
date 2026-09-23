namespace SupplyFlow.Application.DTOs.Reports;

public class LowStockReportDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string SKU { get; set; }
        = string.Empty;

    public string CategoryName { get; set; }
        = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public decimal CurrentQuantity { get; set; }

    public decimal MinimumStockLevel { get; set; }

    public decimal ShortageQuantity { get; set; }
}