using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Reporting.Queries.GetDashboard;

public record GetDashboardQuery : IRequest<Result<DashboardDto>>;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public GetDashboardQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var today = _dateTime.Today;
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        var openMatters = await _context.Matters
            .CountAsync(m => !m.IsDeleted && m.Status != MatterStatus.Closed && m.Status != MatterStatus.Archived, cancellationToken);

        var overdueTasks = await _context.MatterTasks
            .CountAsync(t => !t.IsDeleted && t.DueDate < today && t.Status != TaskItemStatus.Complete && t.Status != TaskItemStatus.Cancelled, cancellationToken);

        var pendingCompliance = await _context.ComplianceChecks
            .CountAsync(c => c.Status == ComplianceCheckStatus.Pending, cancellationToken);

        var unreadNotifications = _currentUser.UserId.HasValue
            ? await _context.Notifications.CountAsync(n => n.UserId == _currentUser.UserId.Value && !n.IsRead, cancellationToken)
            : 0;

        var unbilledTimeValue = await _context.TimeEntries
            .Where(t => !t.IsDeleted && t.Status == TimeEntryStatus.Approved && t.InvoiceId == null && t.IsBillable)
            .SumAsync(t => (t.DurationMinutes / 60.0m) * (t.RateAmount ?? 0), cancellationToken);

        var outstandingInvoices = await _context.Invoices
            .Where(i => !i.IsDeleted && (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid))
            .SumAsync(i => i.TotalAmount - i.PaidAmount, cancellationToken);

        var paidThisMonth = await _context.Payments
            .Where(p => !p.IsReversed && p.PaymentDate >= monthStart)
            .SumAsync(p => p.Amount, cancellationToken);

        var complianceAlerts = await _context.ComplianceChecks
            .CountAsync(c => c.Status == ComplianceCheckStatus.Fail || c.Status == ComplianceCheckStatus.RequiresInvestigation, cancellationToken);

        var draftTimeEntries = await _context.TimeEntries
            .CountAsync(t => !t.IsDeleted && t.Status == TimeEntryStatus.Draft, cancellationToken);

        var pendingApprovals = await _context.TimeEntries
            .CountAsync(t => !t.IsDeleted && t.Status == TimeEntryStatus.Submitted, cancellationToken);

        return Result<DashboardDto>.Success(new DashboardDto
        {
            OpenMattersCount = openMatters,
            OverdueTasksCount = overdueTasks,
            PendingComplianceCount = pendingCompliance,
            UnreadNotificationsCount = unreadNotifications,
            UnbilledTimeValue = unbilledTimeValue,
            OutstandingInvoicesTotal = outstandingInvoices,
            PaidThisMonthTotal = paidThisMonth,
            ComplianceAlertsCount = complianceAlerts,
            DraftTimeEntriesCount = draftTimeEntries,
            PendingApprovalsCount = pendingApprovals
        });
    }
}
