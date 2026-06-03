using Clarity.Application.Invoices.Commands.CreateInvoice;
using Clarity.Application.Invoices.Commands.IssueInvoice;
using Clarity.Application.Invoices.Queries.GetInvoices;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] Guid? clientId,
        [FromQuery] Guid? matterId,
        [FromQuery] InvoiceStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetInvoicesQuery
        {
            ClientId = clientId,
            MatterId = matterId,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}/issue")]
    public async Task<IActionResult> IssueInvoice(Guid id)
    {
        var result = await _mediator.Send(new IssueInvoiceCommand(id));
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
