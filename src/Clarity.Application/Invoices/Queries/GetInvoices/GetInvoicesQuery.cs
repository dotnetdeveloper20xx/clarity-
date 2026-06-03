using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Invoices.Queries.GetInvoices;

public record GetInvoicesQuery : IRequest<Result<PaginatedList<InvoiceDto>>>
{
    public Guid? ClientId { get; init; }
    public Guid? MatterId { get; init; }
    public InvoiceStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
