using Clarity.Application.Clients.Queries.GetClient;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Clients.Queries.GetClients;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, Result<PaginatedList<ClientDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetClientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ClientDto>>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Clients
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.ReferenceNumber.ToLower().Contains(search) ||
                (c.Email != null && c.Email.ToLower().Contains(search)));
        }

        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);

        if (request.ClientType.HasValue)
            query = query.Where(c => c.ClientType == request.ClientType.Value);

        var projected = query.OrderBy(c => c.Name).Select(c => new ClientDto
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
        });

        var result = await PaginatedList<ClientDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<ClientDto>>.Success(result);
    }
}
