using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
