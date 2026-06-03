using Clarity.Domain.Common;
using Clarity.Domain.Enums;

namespace Clarity.Domain.Entities;

public class MatterTask : AuditableEntity
{
    public Guid MatterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AssigneeId { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateOnly DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public Matter Matter { get; set; } = null!;
    public ApplicationUser Assignee { get; set; } = null!;
}
