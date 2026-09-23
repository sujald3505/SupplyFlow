using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Category : BaseEntity
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}