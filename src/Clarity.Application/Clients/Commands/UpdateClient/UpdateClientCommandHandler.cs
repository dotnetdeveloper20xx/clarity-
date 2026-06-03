using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using MediatR;

namespace Clarity.Application.Clients.Commands.UpdateClient;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public UpdateClientCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(new object[] { request.Id }, cancellationToken);

        if (client is null)
            throw new NotFoundException(nameof(Client), request.Id);

        client.Name = request.Name;
        client.ClientType = request.ClientType;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.AddressLine1 = request.AddressLine1;
        client.AddressLine2 = request.AddressLine2;
        client.City = request.City;
        client.County = request.County;
        client.PostCode = request.PostCode;
        client.Country = request.Country;
        client.CompanyNumber = request.CompanyNumber;
        client.DateOfBirth = request.DateOfBirth;
        client.Notes = request.Notes;
        client.ModifiedAt = _dateTime.UtcNow;
        client.ModifiedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
