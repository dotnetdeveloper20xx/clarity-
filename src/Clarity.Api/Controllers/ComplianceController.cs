using Clarity.Application.Common.Interfaces;
using Clarity.Application.Compliance.Commands.CreateComplianceCheck;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComplianceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public ComplianceController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet("checks")]
    public async Task<IActionResult> GetChecks([FromQuery] Guid? clientId, [FromQuery] int? status)
    {
        var query = _context.ComplianceChecks.AsNoTracking().AsQueryable();

        if (clientId.HasValue)
            query = query.Where(c => c.ClientId == clientId.Value);
        if (status.HasValue)
            query = query.Where(c => (int)c.Status == status.Value);

        var checks = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.ClientId,
                ClientName = c.Client.Name,
                c.CheckType,
                c.Status,
                c.RiskLevel,
                c.PerformedById,
                c.PerformedAt,
                c.Notes,
                c.NextReviewDate,
                c.CreatedAt
            })
            .ToListAsync();

        return Ok(checks);
    }

    [HttpPost("checks")]
    public async Task<IActionResult> CreateCheck([FromBody] CreateComplianceCheckCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
}
