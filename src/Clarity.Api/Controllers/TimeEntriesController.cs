using Clarity.Application.TimeEntries.Commands.ApproveTimeEntry;
using Clarity.Application.TimeEntries.Commands.RecordTime;
using Clarity.Application.TimeEntries.Queries.GetTimeEntries;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimeEntriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTimeEntries(
        [FromQuery] Guid? matterId,
        [FromQuery] Guid? userId,
        [FromQuery] TimeEntryStatus? status,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetTimeEntriesQuery
        {
            MatterId = matterId,
            UserId = userId,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> RecordTime([FromBody] RecordTimeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _mediator.Send(new ApproveTimeEntryCommand(id));
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
