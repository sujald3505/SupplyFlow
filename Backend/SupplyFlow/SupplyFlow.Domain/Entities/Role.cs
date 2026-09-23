using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();

    public bool IsActive { get; set; } = true;
}