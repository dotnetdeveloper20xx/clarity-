using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using Clarity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SendAsync(Guid userId, string title, string message, string type = "Info", string? entityType = null, Guid? entityId = null, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = Enum.TryParse<NotificationType>(type, out var nt) ? nt : NotificationType.Info,
            EntityType = entityType,
            EntityId = entityId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SendToRoleAsync(string role, string title, string message, string type = "Info", string? entityType = null, Guid? entityId = null, CancellationToken cancellationToken = default)
    {
        var userIds = await _context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.Role.Name == role)
            .Select(ur => ur.UserId)
            .ToListAsync(cancellationToken);

        var notificationType = Enum.TryParse<NotificationType>(type, out var nt) ? nt : NotificationType.Info;

        foreach (var userId in userIds)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = notificationType,
                EntityType = entityType,
                EntityId = entityId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
