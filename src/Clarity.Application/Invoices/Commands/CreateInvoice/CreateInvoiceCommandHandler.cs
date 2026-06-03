using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Invoices.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public CreateInvoiceCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var matter = await _context.Matters
            .FirstOrDefaultAsync(m => m.Id == request.MatterId && !m.IsDeleted, cancellationToken);

        if (matter is null)
            throw new NotFoundException(nameof(Matter), request.MatterId);

        // Get approved unbilled time entries for this matter
        var unbilledEntries = await _context.TimeEntries
            .Where(t => t.MatterId == request.MatterId
                && t.Status == TimeEntryStatus.Approved
                && t.IsBillable
                && t.InvoiceId == null
                && !t.IsDeleted)
            .ToListAsync(cancellationToken);

        if (!unbilledEntries.Any())
            return Result<Guid>.Failure("No approved unbilled time entries found for this matter.");

        var invoiceNumber = await GenerateInvoiceNumber(cancellationToken);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            ClientId = request.ClientId,
            MatterId = request.MatterId,
            Status = InvoiceStatus.Draft,
            TaxRate = request.TaxRate,
            Notes = request.Notes,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        decimal subTotal = 0;
        int sortOrder = 1;

        foreach (var entry in unbilledEntries)
        {
            var hours = entry.DurationMinutes / 60.0m;
            var rate = entry.RateAmount ?? 0;
            var amount = Math.Round(hours * rate, 2);

            invoice.LineItems.Add(new InvoiceLineItem
            {
                Description = $"{entry.Date:dd/MM/yyyy} - {entry.Description}",
                Quantity = hours,
                UnitPrice = rate,
                Amount = amount,
                TimeEntryId = entry.Id,
                LineType = LineItemType.Time,
                SortOrder = sortOrder++
            });

            entry.InvoiceId = invoice.Id;
            entry.Status = TimeEntryStatus.Billed;
            subTotal += amount;
        }

        invoice.SubTotal = subTotal;
        invoice.TaxAmount = Math.Round(subTotal * (request.TaxRate / 100m), 2);
        invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(invoice.Id);
    }

    private async Task<string> GenerateInvoiceNumber(CancellationToken cancellationToken)
    {
        var count = await _context.Invoices.CountAsync(cancellationToken);
        return $"INV-{(count + 1):D5}";
    }
}
