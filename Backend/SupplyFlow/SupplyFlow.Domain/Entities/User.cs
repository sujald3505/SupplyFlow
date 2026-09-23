using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class User : BaseEntity
{
    public int CompanyId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public Role Role { get; set; } = null!;

    public ICollection<PurchaseRequest> RequestedPurchaseRequests { get; set; }
    = new List<PurchaseRequest>();

    public ICollection<PurchaseRequest> ApprovedPurchaseRequests { get; set; }
        = new List<PurchaseRequest>();

    public ICollection<GoodsReceipt> GoodsReceipts { get; set; }
    = new List<GoodsReceipt>();
}