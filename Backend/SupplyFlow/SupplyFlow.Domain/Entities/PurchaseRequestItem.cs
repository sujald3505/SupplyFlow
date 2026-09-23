using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class PurchaseRequestItem : BaseEntity
{
    public int PurchaseRequestId { get; set; }

    public int ProductId { get; set; }

    public decimal RequestedQuantity { get; set; }

    public string? Remarks { get; set; }

    // Navigation Properties
    public PurchaseRequest PurchaseRequest { get; set; } = null!;

    public Product Product { get; set; } = null!;
}