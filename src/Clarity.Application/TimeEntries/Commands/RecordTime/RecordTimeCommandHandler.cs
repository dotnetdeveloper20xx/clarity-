using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.TimeEntries.Commands.RecordTime;

public class RecordTimeCommandHandler : IRequestHandler<RecordTimeCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public RecordTimeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(RecordTimeCommand request, CancellationToken cancellationToken)
    {
        var matter = await _context.Matters
            .FirstOrDefaultAsync(m => m.Id == request.MatterId && !m.IsDeleted, cancellationToken);

        if (matter is null)
            throw new NotFoundException(nameof(Matter), request.MatterId);

        if (matter.Status == MatterStatus.Closed || matter.Status == MatterStatus.Archived)
            return Result<Guid>.Failure("Cannot record time against a closed or archived matter.");

        if (request.Date > _dateTime.Today)
            return Result<Guid>.Failure("Cannot record time for a future date.");

        decimal? rateAmount = null;
        if (request.IsBillable && request.BillingRateId.HasValue)
        {
            var rate = await _context.BillingRates
                .FirstOrDefaultAsync(r => r.Id == request.BillingRateId.Value && r.IsActive, cancellationToken);
            rateAmount = rate?.HourlyRate;
        }

        var entry = new TimeEntry
        {
            MatterId = request.MatterId,
            UserId = _currentUser.UserId ?? Guid.Empty,
            Date = request.Date,
            DurationMinutes = request.DurationMinutes,
            Description = request.Description,
            IsBillable = request.IsBillable,
            BillingRateId = request.BillingRateId,
            RateAmount = rateAmount,
            Status = TimeEntryStatus.Draft,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entry.Id);
    }
}
