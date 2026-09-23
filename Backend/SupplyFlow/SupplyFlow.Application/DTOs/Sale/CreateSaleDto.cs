namespace SupplyFlow.Application.DTOs.Sale;

public class CreateSaleDto
{
    public int WarehouseId { get; set; }

    public int CustomerId { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public string? Remarks { get; set; }

    public List<CreateSaleItemDto> Items { get; set; }
        = new();
}