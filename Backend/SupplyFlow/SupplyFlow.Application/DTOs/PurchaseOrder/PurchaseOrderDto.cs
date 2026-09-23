namespace SupplyFlow.Application.DTOs.PurchaseOrder;

public class PurchaseOrderDto
{
    public int Id { get; set; }

    public string PurchaseOrderNumber { get; set; }
        = string.Empty;

    public int SupplierId { get; set; }

    public string SupplierName { get; set; }
        = string.Empty;

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public int? PurchaseRequestId { get; set; }

    public string? PurchaseRequestNumber { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseOrderItemDto> Items { get; set; }
        = new();
}