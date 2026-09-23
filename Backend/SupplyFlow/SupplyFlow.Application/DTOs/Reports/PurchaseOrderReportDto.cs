namespace SupplyFlow.Application.DTOs.Reports;

public class PurchaseOrderReportDto
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

    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }


    public string? Remarks { get; set; }
}