using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using MediatR;

namespace Clarity.Application.Clients.Commands.DeleteClient;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public DeleteClientCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(new object[] { request.Id }, cancellationToken);

        if (client is null)
            throw new NotFoundException(nameof(Client), request.Id);

        client.IsDeleted = true;
        client.DeletedAt = _dateTime.UtcNow;
        client.DeletedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
