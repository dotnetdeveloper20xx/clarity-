using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Matters.Commands.UpdateMatterStatus;

public class UpdateMatterStatusCommandHandler : IRequestHandler<UpdateMatterStatusCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public UpdateMatterStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Unit>> Handle(UpdateMatterStatusCommand request, CancellationToken cancellationToken)
    {
        var matter = await _context.Matters
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.IsDeleted, cancellationToken);

        if (matter is null)
            throw new NotFoundException(nameof(Matter), request.Id);

        if (!IsValidTransition(matter.Status, request.NewStatus))
            return Result<Unit>.Failure($"Cannot transition from {matter.Status} to {request.NewStatus}.");

        matter.Status = request.NewStatus;
        matter.ModifiedAt = _dateTime.UtcNow;
        matter.ModifiedBy = _currentUser.UserId ?? Guid.Empty;

        if (request.NewStatus == MatterStatus.Closed || request.NewStatus == MatterStatus.Archived)
        {
            matter.ClosedDate = DateOnly.FromDateTime(_dateTime.UtcNow);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }

    private static bool IsValidTransition(MatterStatus current, MatterStatus target)
    {
        if (current == target)
            return false;

        return (current, target) switch
        {
            (MatterStatus.Open, MatterStatus.InProgress) => true,
            (MatterStatus.Open, MatterStatus.OnHold) => true,
            (MatterStatus.Open, MatterStatus.Closed) => true,
            (MatterStatus.InProgress, MatterStatus.OnHold) => true,
            (MatterStatus.InProgress, MatterStatus.AwaitingClient) => true,
            (MatterStatus.InProgress, MatterStatus.AwaitingThirdParty) => true,
            (MatterStatus.InProgress, MatterStatus.BillingReview) => true,
            (MatterStatus.InProgress, MatterStatus.Closed) => true,
            (MatterStatus.OnHold, MatterStatus.Open) => true,
            (MatterStatus.OnHold, MatterStatus.InProgress) => true,
            (MatterStatus.OnHold, MatterStatus.Closed) => true,
            (MatterStatus.AwaitingClient, MatterStatus.InProgress) => true,
            (MatterStatus.AwaitingClient, MatterStatus.OnHold) => true,
            (MatterStatus.AwaitingThirdParty, MatterStatus.InProgress) => true,
            (MatterStatus.AwaitingThirdParty, MatterStatus.OnHold) => true,
            (MatterStatus.BillingReview, MatterStatus.InProgress) => true,
            (MatterStatus.BillingReview, MatterStatus.Closed) => true,
            (MatterStatus.Closed, MatterStatus.Archived) => true,
            _ => false
        };
    }
}
