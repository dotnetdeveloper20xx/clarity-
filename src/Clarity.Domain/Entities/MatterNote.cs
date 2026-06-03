using Clarity.Domain.Common;

namespace Clarity.Domain.Entities;

public class MatterNote : AuditableEntity
{
    public Guid MatterId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsClientVisible { get; set; }

    // Navigation
    public Matter Matter { get; set; } = null!;
}
