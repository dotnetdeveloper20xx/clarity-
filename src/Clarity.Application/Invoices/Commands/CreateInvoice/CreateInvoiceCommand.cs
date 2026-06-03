using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand : IRequest<Result<Guid>>
{
    public Guid MatterId { get; init; }
    public Guid ClientId { get; init; }
    public decimal TaxRate { get; init; } = 20.00m;
    public string? Notes { get; init; }
}
