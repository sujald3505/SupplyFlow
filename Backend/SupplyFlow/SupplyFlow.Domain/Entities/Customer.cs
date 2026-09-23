using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Customer : BaseEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? GSTIN { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Property
    public Company Company { get; set; } = null!;

    public ICollection<Sale> Sales { get; set; }
        = new List<Sale>();
}