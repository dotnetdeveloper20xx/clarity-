using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Invoices.Queries.GetInvoices;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, Result<PaginatedList<InvoiceDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetInvoicesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<InvoiceDto>>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Invoices.AsNoTracking().Where(i => !i.IsDeleted);

        if (request.ClientId.HasValue)
            query = query.Where(i => i.ClientId == request.ClientId.Value);
        if (request.MatterId.HasValue)
            query = query.Where(i => i.MatterId == request.MatterId.Value);
        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        var projected = query.OrderByDescending(i => i.CreatedAt).Select(i => new InvoiceDto
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            ClientId = i.ClientId,
            ClientName = i.Client.Name,
            MatterId = i.MatterId,
            MatterReference = i.Matter.ReferenceNumber,
            Status = i.Status,
            IssueDate = i.IssueDate,
            DueDate = i.DueDate,
            SubTotal = i.SubTotal,
            TaxAmount = i.TaxAmount,
            TotalAmount = i.TotalAmount,
            PaidAmount = i.PaidAmount
        });

        var result = await PaginatedList<InvoiceDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
        return Result<PaginatedList<InvoiceDto>>.Success(result);
    }
}
