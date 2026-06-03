using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Clarity.Infrastructure.Persistence;

namespace Clarity.Infrastructure.Services;

public class ActivityTimelineService : IActivityTimelineService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ActivityTimelineService(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task RecordAsync(string entityType, Guid entityId, string title, string eventType, string? description = null, string? icon = null, CancellationToken cancellationToken = default)
    {
        var evt = new ActivityEvent
        {
            EntityType = entityType,
            EntityId = entityId,
            Title = title,
            EventType = eventType,
            Description = description,
            Icon = icon,
            PerformedById = _currentUser.UserId,
            PerformedByName = _currentUser.Email,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.Set<ActivityEvent>().Add(evt);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
