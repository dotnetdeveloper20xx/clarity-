using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Clients.Queries.GetClient;

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, Result<ClientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetClientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ClientDto>> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .AsNoTracking()
            .Where(c => c.Id == request.Id && !c.IsDeleted)
            .Select(c => new ClientDto
            {
                Id = c.Id,
                ReferenceNumber = c.ReferenceNumber,
                Name = c.Name,
                ClientType = c.ClientType,
                Status = c.Status,
                Email = c.Email,
                Phone = c.Phone,
                AddressLine1 = c.AddressLine1,
                AddressLine2 = c.AddressLine2,
                City = c.City,
                County = c.County,
                PostCode = c.PostCode,
                Country = c.Country,
                CompanyNumber = c.CompanyNumber,
                DateOfBirth = c.DateOfBirth,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (client is null)
            throw new NotFoundException(nameof(Client), request.Id);

        return Result<ClientDto>.Success(client);
    }
}
