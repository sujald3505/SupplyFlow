namespace SupplyFlow.Application.DTOs.PurchaseOrder;

public class PurchaseOrderItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string SKU { get; set; }
        = string.Empty;

    public decimal OrderedQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal RemainingQuantity =>
        OrderedQuantity - ReceivedQuantity;
}