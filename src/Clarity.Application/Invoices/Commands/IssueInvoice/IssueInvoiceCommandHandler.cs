using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Invoices.Commands.IssueInvoice;

public class IssueInvoiceCommandHandler : IRequestHandler<IssueInvoiceCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public IssueInvoiceCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (invoice is null)
            throw new NotFoundException(nameof(Invoice), request.Id);

        if (invoice.Status != InvoiceStatus.Draft)
            return Result.Failure("Only draft invoices can be issued.");

        if (!invoice.LineItems.Any())
            return Result.Failure("Invoice must have at least one line item.");

        invoice.Status = InvoiceStatus.Issued;
        invoice.IssueDate = _dateTime.Today;
        invoice.DueDate = _dateTime.Today.AddDays(30);
        invoice.ModifiedAt = _dateTime.UtcNow;
        invoice.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
