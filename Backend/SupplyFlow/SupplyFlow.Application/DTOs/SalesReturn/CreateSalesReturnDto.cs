namespace SupplyFlow.Application.DTOs.SalesReturn;

public class CreateSalesReturnDto
{
    public int SaleId { get; set; }

    public int WarehouseId { get; set; }

    public int CustomerId { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public string? Remarks { get; set; }

    public List<CreateSalesReturnItemDto> Items { get; set; }
        = new();
}