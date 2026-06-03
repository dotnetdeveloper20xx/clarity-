using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Clients.Commands.CreateClient;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public CreateClientCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<Guid>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = new Client
        {
            ReferenceNumber = await GenerateReferenceNumber(cancellationToken),
            Name = request.Name,
            ClientType = request.ClientType,
            Status = ClientStatus.Pending,
            Email = request.Email,
            Phone = request.Phone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            County = request.County,
            PostCode = request.PostCode,
            Country = request.Country,
            CompanyNumber = request.CompanyNumber,
            DateOfBirth = request.DateOfBirth,
            Notes = request.Notes,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUser.UserId ?? Guid.Empty
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(client.Id);
    }

    private async Task<string> GenerateReferenceNumber(CancellationToken cancellationToken)
    {
        var count = await Task.FromResult(_context.Clients.Count());
        return $"CLI-{(count + 1):D5}";
    }
}
