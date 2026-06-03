using Clarity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimelineController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public TimelineController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("matter/{matterId}")]
    public async Task<IActionResult> GetMatterTimeline(Guid matterId, [FromQuery] int take = 50)
    {
        // Query audit entries for this matter as timeline
        var events = await _context.AuditEntries
            .Where(a => a.EntityType == "Matter" && a.EntityId == matterId)
            .OrderByDescending(a => a.Timestamp)
            .Take(take)
            .Select(a => new
            {
                a.Id,
                a.Timestamp,
                a.Action,
                a.UserEmail,
                a.OldValues,
                a.NewValues
            })
            .ToListAsync();

        return Ok(events);
    }
}
