using Clarity.Application.Reporting.Queries.GetDashboard;
using Clarity.Application.Reporting.Queries.GetFinancialSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _mediator.Send(new GetDashboardQuery());
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialSummary()
    {
        var result = await _mediator.Send(new GetFinancialSummaryQuery());
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
}
