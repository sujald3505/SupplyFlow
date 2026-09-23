namespace SupplyFlow.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1,
    Sent = 2,
    Confirmed = 3,
    PartiallyReceived = 4,
    Completed = 5,
    Cancelled = 6
}