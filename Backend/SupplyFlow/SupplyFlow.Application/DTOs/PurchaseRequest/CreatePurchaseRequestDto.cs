namespace SupplyFlow.Application.DTOs.PurchaseRequest;

public class CreatePurchaseRequestDto
{
    public string? Remarks { get; set; }

    public List<CreatePurchaseRequestItemDto> Items { get; set; }
        = new();
}