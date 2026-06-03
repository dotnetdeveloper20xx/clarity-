using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Workflows.TimeEntryWorkflow;

public record RejectTimeEntryCommand(Guid Id, string Reason) : IRequest<Result>;

public class RejectTimeEntryCommandHandler : IRequestHandler<RejectTimeEntryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;
    private readonly IAuditService _audit;
    private readonly INotificationService _notifications;

    public RejectTimeEntryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime, IAuditService audit, INotificationService notifications)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _audit = audit;
        _notifications = notifications;
    }

    public async Task<Result> Handle(RejectTimeEntryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure("A reason is required when rejecting a time entry.");

        var entry = await _context.TimeEntries.FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);
        if (entry is null) throw new NotFoundException(nameof(TimeEntry), request.Id);

        if (!TimeEntryStatusTransitionValidator.IsValidTransition(entry.Status, TimeEntryStatus.Rejected))
            return Result.Failure($"Cannot reject a time entry with status '{entry.Status}'.");

        entry.Status = TimeEntryStatus.Rejected;
        entry.RejectionReason = request.Reason;
        entry.ModifiedAt = _dateTime.UtcNow;
        entry.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.RecordAsync("TimeEntry", entry.Id, "Rejected", null, new { Reason = request.Reason }, cancellationToken: cancellationToken);
        await _notifications.SendAsync(entry.UserId, "Time Entry Rejected", $"Your time entry for {entry.Date:dd/MM/yyyy} was rejected: {request.Reason}", "Warning", "TimeEntry", entry.Id, cancellationToken);

        return Result.Success();
    }
}
