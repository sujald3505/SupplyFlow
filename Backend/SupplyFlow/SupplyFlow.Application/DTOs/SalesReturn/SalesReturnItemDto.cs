namespace SupplyFlow.Application.DTOs.SalesReturn;

public class SalesReturnItemDto
{
    public int Id { get; set; }

    public int SaleItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string SKU { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }
}