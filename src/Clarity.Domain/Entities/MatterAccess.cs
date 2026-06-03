namespace Clarity.Domain.Entities;

/// <summary>
/// Controls which users can access specific matters beyond their team membership.
/// </summary>
public class MatterAccess
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MatterId { get; set; }
    public Guid UserId { get; set; }
    public MatterAccessLevel AccessLevel { get; set; } = MatterAccessLevel.Read;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public Guid GrantedBy { get; set; }

    public Matter Matter { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

public enum MatterAccessLevel
{
    Read = 0,
    Contribute = 1,
    Manage = 2,
    Restricted = 3
}
