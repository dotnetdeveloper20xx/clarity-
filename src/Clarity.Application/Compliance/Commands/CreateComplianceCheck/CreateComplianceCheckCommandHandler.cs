using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Compliance.Commands.CreateComplianceCheck;

public class CreateComplianceCheckCommandHandler : IRequestHandler<CreateComplianceCheckCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public CreateComplianceCheckCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(CreateComplianceCheckCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.ClientId && !c.IsDeleted, cancellationToken);

        if (client is null)
            throw new NotFoundException(nameof(Client), request.ClientId);

        var check = new ComplianceCheck
        {
            ClientId = request.ClientId,
            MatterId = request.MatterId,
            CheckType = request.CheckType,
            Status = request.Status,
            RiskLevel = request.RiskLevel,
            Notes = request.Notes,
            PerformedById = _currentUser.UserId,
            PerformedAt = _dateTime.UtcNow,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.ComplianceChecks.Add(check);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(check.Id);
    }
}
