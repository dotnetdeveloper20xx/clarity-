using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Payments.Commands.RecordPayment;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public RecordPaymentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && !i.IsDeleted, cancellationToken);

        if (invoice is null)
            throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<Guid>.Failure("Invoice is already fully paid.");

        if (invoice.Status == InvoiceStatus.Draft)
            return Result<Guid>.Failure("Cannot record payment against a draft invoice.");

        if (request.Amount <= 0)
            return Result<Guid>.Failure("Payment amount must be greater than zero.");

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentDate = request.PaymentDate,
            PaymentMethod = request.PaymentMethod,
            Reference = request.Reference,
            Notes = request.Notes,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.Payments.Add(payment);

        // Update invoice paid amount and status
        invoice.PaidAmount += request.Amount;
        if (invoice.PaidAmount >= invoice.TotalAmount)
            invoice.Status = InvoiceStatus.Paid;
        else
            invoice.Status = InvoiceStatus.PartiallyPaid;

        invoice.ModifiedAt = _dateTime.UtcNow;
        invoice.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(payment.Id);
    }
}
