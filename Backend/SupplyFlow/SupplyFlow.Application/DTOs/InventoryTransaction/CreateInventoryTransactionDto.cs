using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Application.DTOs.InventoryTransaction;

public class CreateInventoryTransactionDto
{
    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public InventoryTransactionType TransactionType { get; set; }

    public decimal Quantity { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Remarks { get; set; }
}