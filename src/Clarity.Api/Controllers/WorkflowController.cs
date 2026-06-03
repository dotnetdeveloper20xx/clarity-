using Clarity.Application.Workflows.MatterWorkflow;
using Clarity.Application.Workflows.TimeEntryWorkflow;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkflowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("matters/{id}/transition")]
    public async Task<IActionResult> TransitionMatter(Guid id, [FromBody] ChangeMatterStatusCommand command)
    {
        if (id != command.MatterId)
            return BadRequest("Route ID does not match body MatterId.");

        var result = await _mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("time-entries/{id}/submit")]
    public async Task<IActionResult> SubmitTimeEntry(Guid id)
    {
        var result = await _mediator.Send(new SubmitTimeEntryCommand(id));
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("time-entries/{id}/reject")]
    public async Task<IActionResult> RejectTimeEntry(Guid id, [FromBody] RejectRequest request)
    {
        var result = await _mediator.Send(new RejectTimeEntryCommand(id, request.Reason));
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}

public record RejectRequest
{
    public string Reason { get; init; } = string.Empty;
}
