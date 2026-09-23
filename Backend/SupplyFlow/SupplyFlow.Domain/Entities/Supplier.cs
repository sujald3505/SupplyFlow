using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Supplier : BaseEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ContactPerson { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? GSTNumber { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public Company Company { get; set; } = null!;

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    = new List<PurchaseOrder>();
}