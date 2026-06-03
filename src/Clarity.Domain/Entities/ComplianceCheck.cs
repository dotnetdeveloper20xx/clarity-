using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class ComplianceCheck : BaseEntity
{
    public Guid ClientId { get; set; }
    public Guid? MatterId { get; set; }
    public ComplianceCheckType CheckType { get; set; }
    public ComplianceCheckStatus Status { get; set; } = ComplianceCheckStatus.Pending;
    public RiskLevel? RiskLevel { get; set; }
    public Guid? PerformedById { get; set; }
    public DateTime? PerformedAt { get; set; }
    public string? Notes { get; set; }
    public DateOnly? NextReviewDate { get; set; }

    // Navigation
    public Client Client { get; set; } = null!;
    public Matter? Matter { get; set; }
    public ApplicationUser? PerformedBy { get; set; }
}
