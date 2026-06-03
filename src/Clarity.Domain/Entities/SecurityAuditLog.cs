namespace Clarity.Domain.Entities;

/// <summary>
/// Tracks security-sensitive events: logins, lockouts, permission changes.
/// </summary>
public class SecurityAuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty; // LoginSuccess, LoginFailed, Lockout, PasswordChanged, RoleAssigned, SessionRevoked
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Details { get; set; }
    public bool IsSuccess { get; set; }
}
