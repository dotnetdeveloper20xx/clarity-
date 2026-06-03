namespace Clarity.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendAsync(Guid userId, string title, string message, string type = "Info", string? entityType = null, Guid? entityId = null, CancellationToken cancellationToken = default);
    Task SendToRoleAsync(string role, string title, string message, string type = "Info", string? entityType = null, Guid? entityId = null, CancellationToken cancellationToken = default);
}
