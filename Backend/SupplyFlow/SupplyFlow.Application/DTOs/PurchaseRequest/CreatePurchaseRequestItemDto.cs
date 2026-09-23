namespace SupplyFlow.Application.DTOs.PurchaseRequest;

public class CreatePurchaseRequestItemDto
{
    public int ProductId { get; set; }

    public decimal RequestedQuantity { get; set; }

    public string? Remarks { get; set; }
}