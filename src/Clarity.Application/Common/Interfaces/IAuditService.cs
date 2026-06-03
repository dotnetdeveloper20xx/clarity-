namespace Clarity.Application.Common.Interfaces;

public interface IAuditService
{
    Task RecordAsync(string entityType, Guid entityId, string action, object? oldValues = null, object? newValues = null, string? reason = null, CancellationToken cancellationToken = default);
}
