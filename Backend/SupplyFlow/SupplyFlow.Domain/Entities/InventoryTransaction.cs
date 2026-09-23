using SupplyFlow.Domain.Common;
using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public int CompanyId { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public InventoryTransactionType TransactionType { get; set; }

    public decimal Quantity { get; set; }

    public decimal PreviousQuantity { get; set; }

    public decimal NewQuantity { get; set; }

    public string? ReferenceNumber { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public Product Product { get; set; } = null!;
}