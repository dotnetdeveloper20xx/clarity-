using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Matters.Commands.CreateMatter;

public class CreateMatterCommandHandler : IRequestHandler<CreateMatterCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public CreateMatterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(CreateMatterCommand request, CancellationToken cancellationToken)
    {
        var matter = new Matter
        {
            ReferenceNumber = await GenerateReferenceNumber(cancellationToken),
            ClientId = request.ClientId,
            Title = request.Title,
            Description = request.Description,
            MatterType = request.MatterType,
            Status = MatterStatus.Open,
            FeeArrangement = request.FeeArrangement,
            EstimatedValue = request.EstimatedValue,
            OpenedDate = DateOnly.FromDateTime(_dateTime.UtcNow),
            LeadConsultantId = request.LeadConsultantId,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.Matters.Add(matter);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(matter.Id);
    }

    private async Task<string> GenerateReferenceNumber(CancellationToken cancellationToken)
    {
        var count = await Task.FromResult(_context.Matters.Count());
        return $"MAT-{(count + 1):D5}";
    }
}
