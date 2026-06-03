using Clarity.Domain.Common;

namespace Clarity.Domain.Entities;

public class MatterTeamMember : BaseEntity
{
    public Guid MatterId { get; set; }
    public Guid UserId { get; set; }
    public string? Role { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid AssignedBy { get; set; }

    // Navigation
    public Matter Matter { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
