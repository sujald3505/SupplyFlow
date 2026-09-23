namespace SupplyFlow.Application.DTOs.PurchaseRequest;

public class PurchaseRequestItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string SKU { get; set; }
        = string.Empty;

    public decimal RequestedQuantity { get; set; }

    public string UnitName { get; set; }
        = string.Empty;

    public string UnitSymbol { get; set; }
        = string.Empty;

    public string? Remarks { get; set; }
}