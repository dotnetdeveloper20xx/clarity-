using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.TimeEntries.Commands.ApproveTimeEntry;

public class ApproveTimeEntryCommandHandler : IRequestHandler<ApproveTimeEntryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public ApproveTimeEntryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(ApproveTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.TimeEntries
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (entry is null)
            throw new NotFoundException(nameof(TimeEntry), request.Id);

        if (entry.Status != TimeEntryStatus.Draft && entry.Status != TimeEntryStatus.Submitted)
            return Result.Failure("Only draft or submitted time entries can be approved.");

        entry.Status = TimeEntryStatus.Approved;
        entry.ApprovedById = _currentUser.UserId;
        entry.ApprovedAt = _dateTime.UtcNow;
        entry.ModifiedAt = _dateTime.UtcNow;
        entry.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
