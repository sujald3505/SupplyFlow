namespace SupplyFlow.Application.DTOs.PurchaseOrder;

public class CreatePurchaseOrderDto
{
    public int SupplierId { get; set; }

    public int WarehouseId { get; set; }

    public int? PurchaseRequestId { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public string? Remarks { get; set; }

    public List<CreatePurchaseOrderItemDto> Items { get; set; }
        = new();
}