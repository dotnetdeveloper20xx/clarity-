using Clarity.Application.Common.Interfaces;
using Clarity.Application.Payments.Commands.RecordPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public PaymentsController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments([FromQuery] Guid? invoiceId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = _context.Payments.AsNoTracking().Where(p => !p.IsReversed);

        if (invoiceId.HasValue)
            query = query.Where(p => p.InvoiceId == invoiceId.Value);

        var payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.InvoiceId,
                InvoiceNumber = p.Invoice.InvoiceNumber,
                ClientName = p.Invoice.Client.Name,
                p.Amount,
                p.PaymentDate,
                PaymentMethod = p.PaymentMethod.ToString(),
                p.Reference,
                p.Notes,
                p.CreatedAt
            })
            .ToListAsync();

        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
}
