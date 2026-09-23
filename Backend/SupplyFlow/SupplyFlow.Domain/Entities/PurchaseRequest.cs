using SupplyFlow.Domain.Common;
using SupplyFlow.Domain.Enums;

namespace SupplyFlow.Domain.Entities;

public class PurchaseRequest : BaseEntity
{
    public int CompanyId { get; set; }

    public string RequestNumber { get; set; } = string.Empty;

    public int RequestedByUserId { get; set; }

    public PurchaseRequestStatus Status { get; set; }
        = PurchaseRequestStatus.Draft;

    public string? Remarks { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int? ApprovedByUserId { get; set; }

    public string? RejectionReason { get; set; }

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public User RequestedByUser { get; set; } = null!;

    public User? ApprovedByUser { get; set; }

    public ICollection<PurchaseRequestItem> Items { get; set; }
        = new List<PurchaseRequestItem>();
}