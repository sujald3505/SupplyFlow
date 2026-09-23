namespace SupplyFlow.Application.DTOs.Sale;

public class SaleDto
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; }
        = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public int CustomerId { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal GrandTotal { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public string? Remarks { get; set; }

    public List<SaleItemDto> Items { get; set; }
        = new();
}