namespace Clarity.Application.Common.Interfaces;

public interface IActivityTimelineService
{
    Task RecordAsync(string entityType, Guid entityId, string title, string eventType, string? description = null, string? icon = null, CancellationToken cancellationToken = default);
}
