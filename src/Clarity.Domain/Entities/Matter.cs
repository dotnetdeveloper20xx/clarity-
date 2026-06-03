using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class Matter : AuditableEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MatterType MatterType { get; set; }
    public MatterStatus Status { get; set; } = MatterStatus.Open;
    public FeeArrangement FeeArrangement { get; set; } = FeeArrangement.Hourly;
    public decimal? EstimatedValue { get; set; }
    public decimal? FixedFeeAmount { get; set; }
    public DateOnly OpenedDate { get; set; }
    public DateOnly? ClosedDate { get; set; }
    public Guid LeadConsultantId { get; set; }

    // Navigation properties
    public Client Client { get; set; } = null!;
    public ApplicationUser LeadConsultant { get; set; } = null!;
    public ICollection<MatterNote> Notes { get; set; } = new List<MatterNote>();
    public ICollection<MatterTask> Tasks { get; set; } = new List<MatterTask>();
    public ICollection<MatterTeamMember> TeamMembers { get; set; } = new List<MatterTeamMember>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();
}
