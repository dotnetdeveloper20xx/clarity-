using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Workflows.MatterWorkflow;

public class ChangeMatterStatusCommandHandler : IRequestHandler<ChangeMatterStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;
    private readonly IAuditService _audit;
    private readonly IActivityTimelineService _timeline;
    private readonly INotificationService _notifications;

    public ChangeMatterStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime,
        IAuditService audit,
        IActivityTimelineService timeline,
        INotificationService notifications)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _audit = audit;
        _timeline = timeline;
        _notifications = notifications;
    }

    public async Task<Result> Handle(ChangeMatterStatusCommand request, CancellationToken cancellationToken)
    {
        var matter = await _context.Matters
            .Include(m => m.Invoices)
            .Include(m => m.ComplianceChecks)
            .FirstOrDefaultAsync(m => m.Id == request.MatterId && !m.IsDeleted, cancellationToken);

        if (matter is null)
            throw new NotFoundException(nameof(Matter), request.MatterId);

        var oldStatus = matter.Status;

        // Validate transition
        if (!MatterStatusTransitionValidator.IsValidTransition(oldStatus, request.NewStatus))
            return Result.Failure($"Cannot transition from {oldStatus} to {request.NewStatus}.");

        // Business rule: closing requires no unpaid invoices
        if (request.NewStatus == MatterStatus.Closed)
        {
            var hasUnpaidInvoices = matter.Invoices.Any(i =>
                !i.IsDeleted && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.WrittenOff && i.Status != InvoiceStatus.Cancelled);

            if (hasUnpaidInvoices)
                return Result.Failure("Matter cannot be closed while there are unpaid invoices. Please resolve outstanding invoices first.");

            // Check compliance
            var hasFailedCompliance = matter.ComplianceChecks.Any(c =>
                c.Status == ComplianceCheckStatus.Fail || c.Status == ComplianceCheckStatus.RequiresInvestigation);

            if (hasFailedCompliance)
                return Result.Failure("Matter cannot be closed with unresolved compliance issues.");

            // Permission check: only TeamLeader or Admin can close
            if (!_currentUser.Roles.Any(r => r == "TeamLeader" || r == "Admin"))
                return Result.Failure("Only Team Leaders or Administrators can close matters.");

            matter.ClosedDate = _dateTime.Today;
        }

        // Apply status change
        matter.Status = request.NewStatus;
        matter.ModifiedAt = _dateTime.UtcNow;
        matter.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        // Record audit
        await _audit.RecordAsync("Matter", matter.Id, "StatusChanged",
            new { Status = oldStatus.ToString() },
            new { Status = request.NewStatus.ToString() },
            request.Reason, cancellationToken);

        // Record timeline event
        await _timeline.RecordAsync("Matter", matter.Id,
            $"Status changed to {request.NewStatus}",
            "StatusChanged",
            request.Reason,
            "🔄", cancellationToken);

        // Notify lead consultant if matter is closed
        if (request.NewStatus == MatterStatus.Closed)
        {
            await _notifications.SendAsync(matter.LeadConsultantId,
                "Matter Closed",
                $"Matter {matter.ReferenceNumber} has been closed.",
                "Info", "Matter", matter.Id, cancellationToken);
        }

        return Result.Success();
    }
}
