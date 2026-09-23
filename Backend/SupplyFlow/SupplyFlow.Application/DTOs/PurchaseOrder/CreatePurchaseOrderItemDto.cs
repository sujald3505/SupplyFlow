namespace SupplyFlow.Application.DTOs.PurchaseOrder;

public class CreatePurchaseOrderItemDto
{
    public int ProductId { get; set; }

    public decimal OrderedQuantity { get; set; }

    public decimal UnitPrice { get; set; }
}