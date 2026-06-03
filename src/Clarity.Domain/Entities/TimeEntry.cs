using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class TimeEntry : AuditableEntity
{
    public Guid MatterId { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public int DurationMinutes { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsBillable { get; set; } = true;
    public Guid? BillingRateId { get; set; }
    public decimal? RateAmount { get; set; }
    public TimeEntryStatus Status { get; set; } = TimeEntryStatus.Draft;
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation
    public Matter Matter { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public BillingRate? BillingRate { get; set; }
    public Invoice? Invoice { get; set; }
}
