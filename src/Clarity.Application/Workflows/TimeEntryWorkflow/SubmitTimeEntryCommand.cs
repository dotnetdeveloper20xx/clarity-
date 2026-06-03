using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Workflows.TimeEntryWorkflow;

public record SubmitTimeEntryCommand(Guid Id) : IRequest<Result>;

public class SubmitTimeEntryCommandHandler : IRequestHandler<SubmitTimeEntryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;
    private readonly IAuditService _audit;

    public SubmitTimeEntryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime, IAuditService audit)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _audit = audit;
    }

    public async Task<Result> Handle(SubmitTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.TimeEntries.FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);
        if (entry is null) throw new NotFoundException(nameof(TimeEntry), request.Id);

        if (!TimeEntryStatusTransitionValidator.IsValidTransition(entry.Status, TimeEntryStatus.Submitted))
            return Result.Failure($"Cannot submit a time entry with status '{entry.Status}'.");

        var oldStatus = entry.Status;
        entry.Status = TimeEntryStatus.Submitted;
        entry.ModifiedAt = _dateTime.UtcNow;
        entry.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.RecordAsync("TimeEntry", entry.Id, "Submitted", new { Status = oldStatus.ToString() }, new { Status = "Submitted" }, cancellationToken: cancellationToken);

        return Result.Success();
    }
}
