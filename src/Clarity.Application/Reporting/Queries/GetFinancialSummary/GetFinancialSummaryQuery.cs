using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Reporting.Queries.GetFinancialSummary;

public record GetFinancialSummaryQuery : IRequest<Result<FinancialSummaryDto>>;

public class GetFinancialSummaryQueryHandler : IRequestHandler<GetFinancialSummaryQuery, Result<FinancialSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeService _dateTime;

    public GetFinancialSummaryQueryHandler(IApplicationDbContext context, IDateTimeService dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<Result<FinancialSummaryDto>> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = _dateTime.Today;
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        // Total billed this month (invoices issued this month)
        var billedThisMonth = await _context.Invoices
            .Where(i => !i.IsDeleted && i.IssueDate >= monthStart && i.Status != InvoiceStatus.Draft)
            .SumAsync(i => i.TotalAmount, cancellationToken);

        // Total paid this month
        var paidThisMonth = await _context.Payments
            .Where(p => !p.IsReversed && p.PaymentDate >= monthStart)
            .SumAsync(p => p.Amount, cancellationToken);

        // Total outstanding
        var outstanding = await _context.Invoices
            .Where(i => !i.IsDeleted && (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid))
            .SumAsync(i => i.TotalAmount - i.PaidAmount, cancellationToken);

        // WIP (approved unbilled time)
        var wip = await _context.TimeEntries
            .Where(t => !t.IsDeleted && t.Status == TimeEntryStatus.Approved && t.InvoiceId == null && t.IsBillable)
            .SumAsync(t => (t.DurationMinutes / 60.0m) * (t.RateAmount ?? 0), cancellationToken);

        // Aged debt
        var outstandingInvoices = await _context.Invoices
            .Where(i => !i.IsDeleted && (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid) && i.DueDate.HasValue)
            .Select(i => new { i.DueDate, Outstanding = i.TotalAmount - i.PaidAmount })
            .ToListAsync(cancellationToken);

        var agedDebt = outstandingInvoices
            .GroupBy(i => GetAgeBand(i.DueDate!.Value, today))
            .Select(g => new AgedDebtBand { Band = g.Key, Amount = g.Sum(x => x.Outstanding), InvoiceCount = g.Count() })
            .OrderBy(b => GetBandOrder(b.Band))
            .ToList();

        // Top clients by revenue (payments this year)
        var yearStart = new DateOnly(today.Year, 1, 1);
        var topClients = await _context.Payments
            .Where(p => !p.IsReversed && p.PaymentDate >= yearStart)
            .Join(_context.Invoices, p => p.InvoiceId, i => i.Id, (p, i) => new { p.Amount, i.ClientId })
            .Join(_context.Clients, x => x.ClientId, c => c.Id, (x, c) => new { x.Amount, c.Id, c.Name })
            .GroupBy(x => new { x.Id, x.Name })
            .Select(g => new TopClientRevenue { ClientId = g.Key.Id, ClientName = g.Key.Name, Revenue = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Revenue)
            .Take(5)
            .ToListAsync(cancellationToken);

        return Result<FinancialSummaryDto>.Success(new FinancialSummaryDto
        {
            TotalBilledThisMonth = billedThisMonth,
            TotalPaidThisMonth = paidThisMonth,
            TotalOutstanding = outstanding,
            TotalWip = wip,
            AgedDebt = agedDebt,
            TopClients = topClients
        });
    }

    private static string GetAgeBand(DateOnly dueDate, DateOnly today)
    {
        var days = today.DayNumber - dueDate.DayNumber;
        return days switch
        {
            <= 0 => "Current",
            <= 30 => "1-30 Days",
            <= 60 => "31-60 Days",
            <= 90 => "61-90 Days",
            _ => "90+ Days"
        };
    }

    private static int GetBandOrder(string band) => band switch
    {
        "Current" => 0,
        "1-30 Days" => 1,
        "31-60 Days" => 2,
        "61-90 Days" => 3,
        _ => 4
    };
}
