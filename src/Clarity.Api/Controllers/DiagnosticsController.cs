using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Support")]
public class DiagnosticsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public DiagnosticsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetBackgroundJobs([FromQuery] string? status, [FromQuery] int take = 50)
    {
        var query = _context.Set<BackgroundJob>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BackgroundJobStatus>(status, true, out var s))
            query = query.Where(j => j.Status == s);

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Take(take)
            .Select(j => new
            {
                j.Id,
                j.JobType,
                Status = j.Status.ToString(),
                j.RetryCount,
                j.MaxRetries,
                j.ErrorMessage,
                j.CreatedAt,
                j.StartedAt,
                j.CompletedAt
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("jobs/summary")]
    public async Task<IActionResult> GetJobsSummary()
    {
        var pending = await _context.Set<BackgroundJob>().CountAsync(j => j.Status == BackgroundJobStatus.Pending);
        var processing = await _context.Set<BackgroundJob>().CountAsync(j => j.Status == BackgroundJobStatus.Processing);
        var failed = await _context.Set<BackgroundJob>().CountAsync(j => j.Status == BackgroundJobStatus.Failed);
        var deadLetter = await _context.Set<BackgroundJob>().CountAsync(j => j.Status == BackgroundJobStatus.DeadLetter);

        return Ok(new { pending, processing, failed, deadLetter });
    }

    [HttpPost("jobs/{id}/retry")]
    public async Task<IActionResult> RetryJob(Guid id)
    {
        var job = await _context.Set<BackgroundJob>().FindAsync(id);
        if (job is null) return NotFound();

        if (job.Status != BackgroundJobStatus.Failed && job.Status != BackgroundJobStatus.DeadLetter)
            return BadRequest("Only failed or dead-letter jobs can be retried.");

        job.Status = BackgroundJobStatus.Pending;
        job.RetryCount = 0;
        job.ErrorMessage = null;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("recent-errors")]
    public async Task<IActionResult> GetRecentErrors([FromQuery] int take = 20)
    {
        var errors = await _context.AuditEntries
            .AsNoTracking()
            .Where(a => a.Action == "Error" || a.Action == "Exception")
            .OrderByDescending(a => a.Timestamp)
            .Take(take)
            .Select(a => new { a.Id, a.Timestamp, a.UserEmail, a.Action, a.EntityType, a.CorrelationId, a.NewValues })
            .ToListAsync();

        return Ok(errors);
    }

    [HttpGet("system-info")]
    public IActionResult GetSystemInfo()
    {
        return Ok(new
        {
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            MachineName = Environment.MachineName,
            Runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            ServerTime = DateTime.UtcNow,
            Uptime = Environment.TickCount64 / 1000 / 60 + " minutes"
        });
    }
}
