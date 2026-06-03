using Clarity.Domain.Common;

namespace Clarity.Domain.Entities;

/// <summary>
/// Represents a human-readable timeline event for matters and clients.
/// Unlike AuditEntry (for accountability), ActivityEvent is for user understanding.
/// </summary>
public class ActivityEvent : BaseEntity
{
    public string EntityType { get; set; } = string.Empty; // "Matter", "Client"
    public Guid EntityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EventType { get; set; } = string.Empty; // StatusChanged, DocumentUploaded, TimeApproved, etc.
    public string? Icon { get; set; }
    public Guid? PerformedById { get; set; }
    public string? PerformedByName { get; set; }
}
