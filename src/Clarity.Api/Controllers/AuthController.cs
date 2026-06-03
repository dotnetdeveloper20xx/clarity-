using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !user.IsActive)
            return Unauthorized(new { message = "Invalid login details." });

        // Check lockout
        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            return Unauthorized(new { message = "Account is locked. Please try again later." });

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.IsLockedOut = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                // Log security event
                _context.Set<SecurityAuditLog>().Add(new SecurityAuditLog
                {
                    EventType = "Lockout",
                    UserId = user.Id,
                    UserEmail = user.Email,
                    IpAddress = GetIpAddress(),
                    Details = $"Account locked after {user.FailedLoginAttempts} failed attempts",
                    IsSuccess = false
                });
            }

            // Log failed login
            _context.Set<SecurityAuditLog>().Add(new SecurityAuditLog
            {
                EventType = "LoginFailed",
                UserId = user.Id,
                UserEmail = user.Email,
                IpAddress = GetIpAddress(),
                IsSuccess = false
            });

            await _context.SaveChangesAsync();
            return Unauthorized(new { message = "Invalid login details." });
        }

        // Successful login — reset lockout
        user.FailedLoginAttempts = 0;
        user.IsLockedOut = false;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            DeviceInfo = Request.Headers["User-Agent"].FirstOrDefault(),
            IpAddress = GetIpAddress()
        };
        _context.Set<RefreshToken>().Add(refreshTokenEntity);

        // Create session
        _context.Set<UserSession>().Add(new UserSession
        {
            UserId = user.Id,
            RefreshTokenId = refreshTokenEntity.Id,
            DeviceInfo = Request.Headers["User-Agent"].FirstOrDefault(),
            IpAddress = GetIpAddress()
        });

        // Log success
        _context.Set<SecurityAuditLog>().Add(new SecurityAuditLog
        {
            EventType = "LoginSuccess",
            UserId = user.Id,
            UserEmail = user.Email,
            IpAddress = GetIpAddress(),
            DeviceInfo = Request.Headers["User-Agent"].FirstOrDefault(),
            IsSuccess = true
        });

        await _context.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var storedToken = await _context.Set<RefreshToken>()
            .Include(rt => rt.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken is null || !storedToken.IsActive)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        var user = storedToken.User;
        if (!user.IsActive)
            return Unauthorized(new { message = "Account is disabled." });

        // Rotate refresh token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = GenerateRefreshToken();
        storedToken.ReplacedByToken = newRefreshToken;

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            DeviceInfo = Request.Headers["User-Agent"].FirstOrDefault(),
            IpAddress = GetIpAddress()
        };
        _context.Set<RefreshToken>().Add(newRefreshTokenEntity);

        var accessToken = GenerateAccessToken(user);
        await _context.SaveChangesAsync();

        return Ok(new LoginResponse
        {
            Token = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request)
    {
        if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            var token = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (token is not null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

        if (user is null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        });
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<IActionResult> GetMySessions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var sessions = await _context.Set<UserSession>()
            .Where(s => s.UserId == Guid.Parse(userId) && !s.IsRevoked)
            .OrderByDescending(s => s.LastActivityAt)
            .Select(s => new { s.Id, s.DeviceInfo, s.IpAddress, s.CreatedAt, s.LastActivityAt })
            .ToListAsync();

        return Ok(sessions);
    }

    [HttpPost("sessions/{id}/revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeSession(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var session = await _context.Set<UserSession>()
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == Guid.Parse(userId));

        if (session is null) return NotFound();

        session.IsRevoked = true;
        session.RevokedAt = DateTime.UtcNow;

        // Also revoke the associated refresh token
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

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user is null) return NotFound();

        if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Current password is incorrect." });

        user.PasswordHash = HashPassword(request.NewPassword);
        user.PasswordChangedAt = DateTime.UtcNow;

        _context.Set<SecurityAuditLog>().Add(new SecurityAuditLog
        {
            EventType = "PasswordChanged",
            UserId = user.Id,
            UserEmail = user.Email,
            IpAddress = GetIpAddress(),
            IsSuccess = true
        });

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "ClarityDevelopmentSecretKey12345678"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        foreach (var role in user.UserRoles)
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "Clarity",
            audience: _configuration["Jwt:Audience"] ?? "ClarityUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes) == hash;
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private string? GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}

public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
}

public record RefreshRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record LogoutRequest
{
    public string? RefreshToken { get; init; }
}

public record ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
