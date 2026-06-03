namespace Clarity.Domain.Enums;

public enum MatterStatus
{
    Open = 0,
    InProgress = 1,
    OnHold = 2,
    AwaitingClient = 3,
    AwaitingThirdParty = 4,
    BillingReview = 5,
    Closed = 6,
    Archived = 7
}
