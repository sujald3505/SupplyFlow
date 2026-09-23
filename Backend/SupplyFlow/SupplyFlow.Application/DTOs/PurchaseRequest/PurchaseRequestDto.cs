namespace SupplyFlow.Application.DTOs.PurchaseRequest;

public class PurchaseRequestDto
{
    public int Id { get; set; }

    public string RequestNumber { get; set; }
        = string.Empty;

    public int RequestedByUserId { get; set; }

    public string RequestedByUserName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public string? Remarks { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public int? ApprovedByUserId { get; set; }

    public string? ApprovedByUserName { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PurchaseRequestItemDto> Items { get; set; }
        = new();
}