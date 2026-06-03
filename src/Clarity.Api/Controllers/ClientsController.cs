using Clarity.Application.Clients.Commands.CreateClient;
using Clarity.Application.Clients.Commands.DeleteClient;
using Clarity.Application.Clients.Commands.UpdateClient;
using Clarity.Application.Clients.Queries.GetClient;
using Clarity.Application.Clients.Queries.GetClients;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients(
        [FromQuery] string? searchTerm,
        [FromQuery] ClientStatus? status,
        [FromQuery] ClientType? clientType,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetClientsQuery
        {
            SearchTerm = searchTerm,
            Status = status,
            ClientType = clientType,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClient(Guid id)
    {
        var result = await _mediator.Send(new GetClientQuery(id));
        return result.Succeeded ? Ok(result.Data) : NotFound(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded
            ? CreatedAtAction(nameof(GetClient), new { id = result.Data }, result.Data)
            : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateClient(Guid id, [FromBody] UpdateClientCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        var result = await _mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClient(Guid id)
    {
        var result = await _mediator.Send(new DeleteClientCommand(id));
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
