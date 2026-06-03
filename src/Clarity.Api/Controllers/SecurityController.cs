using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SecurityController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public SecurityController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("audit-log")]
    public async Task<IActionResult> GetSecurityAuditLog(
        [FromQuery] string? eventType,
        [FromQuery] Guid? userId,
        [FromQuery] bool? successOnly,
        [FromQuery] int take = 50)
    {
        var query = _context.Set<SecurityAuditLog>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(s => s.EventType == eventType);
        if (userId.HasValue)
            query = query.Where(s => s.UserId == userId.Value);
        if (successOnly.HasValue)
            query = query.Where(s => s.IsSuccess == successOnly.Value);

        var logs = await query
            .OrderByDescending(s => s.Timestamp)
            .Take(take)
            .Select(s => new
            {
                s.Id,
                s.Timestamp,
                s.EventType,
                s.UserId,
                s.UserEmail,
                s.IpAddress,
                s.DeviceInfo,
                s.Details,
                s.IsSuccess
            })
            .ToListAsync();

        return Ok(logs);
    }

    [HttpGet("active-sessions")]
    public async Task<IActionResult> GetAllActiveSessions()
    {
        var sessions = await _context.Set<UserSession>()
            .AsNoTracking()
            .Where(s => !s.IsRevoked)
            .OrderByDescending(s => s.LastActivityAt)
            .Take(100)
            .Select(s => new
            {
                s.Id,
                s.UserId,
                UserEmail = s.User.Email,
                s.DeviceInfo,
                s.IpAddress,
                s.CreatedAt,
                s.LastActivityAt
            })
            .ToListAsync();

        return Ok(sessions);
    }

    [HttpPost("sessions/{id}/revoke")]
    public async Task<IActionResult> AdminRevokeSession(Guid id)
    {
        var session = await _context.Set<UserSession>().FindAsync(id);
        if (session is null) return NotFound();

        session.IsRevoked = true;
        session.RevokedAt = DateTime.UtcNow;

        var refreshToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Id == session.RefreshTokenId);
        if (refreshToken is not null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("locked-accounts")]
    public async Task<IActionResult> GetLockedAccounts()
    {
        var locked = await _context.Users
            .AsNoTracking()
            .Where(u => u.IsLockedOut)
            .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName, u.LockoutEnd, u.FailedLoginAttempts })
            .ToListAsync();

        return Ok(locked);
    }

    [HttpPost("users/{id}/unlock")]
    public async Task<IActionResult> UnlockAccount(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.IsLockedOut = false;
        user.LockoutEnd = null;
        user.FailedLoginAttempts = 0;

        _context.Set<SecurityAuditLog>().Add(new SecurityAuditLog
        {
            EventType = "AccountUnlocked",
            UserId = user.Id,
            UserEmail = user.Email,
            Details = "Unlocked by administrator",
            IsSuccess = true
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
