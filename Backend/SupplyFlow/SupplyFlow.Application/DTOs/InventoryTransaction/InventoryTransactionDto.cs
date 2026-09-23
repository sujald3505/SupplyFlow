namespace SupplyFlow.Application.DTOs.InventoryTransaction;

public class InventoryTransactionDto
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; }
        = string.Empty;

    public int ProductId { get; set; }

    public string ProductName { get; set; }
        = string.Empty;

    public string SKU { get; set; }
        = string.Empty;

    public string TransactionType { get; set; }
        = string.Empty;

    public decimal Quantity { get; set; }

    public decimal PreviousQuantity { get; set; }

    public decimal NewQuantity { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }
}