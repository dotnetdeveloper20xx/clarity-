using System.Text.Json;
using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Clarity.Infrastructure.Persistence;

namespace Clarity.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditService(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task RecordAsync(string entityType, Guid entityId, string action, object? oldValues = null, object? newValues = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        var entry = new AuditEntry
        {
            Timestamp = DateTime.UtcNow,
            UserId = _currentUser.UserId ?? Guid.Empty,
            UserEmail = _currentUser.Email ?? "system",
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues is not null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues is not null ? JsonSerializer.Serialize(newValues) : null,
        };

        _context.AuditEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
