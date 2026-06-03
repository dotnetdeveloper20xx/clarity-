using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Application.Matters.Queries.GetMatter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Matters.Queries.GetMatters;

public class GetMattersQueryHandler : IRequestHandler<GetMattersQuery, Result<PaginatedList<MatterDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMattersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<MatterDto>>> Handle(GetMattersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Matters
            .AsNoTracking()
            .Where(m => !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(m =>
                m.Title.ToLower().Contains(search) ||
                m.ReferenceNumber.ToLower().Contains(search) ||
                m.Client.Name.ToLower().Contains(search));
        }

        if (request.Status.HasValue)
            query = query.Where(m => m.Status == request.Status.Value);

        if (request.MatterType.HasValue)
            query = query.Where(m => m.MatterType == request.MatterType.Value);

        if (request.ClientId.HasValue)
            query = query.Where(m => m.ClientId == request.ClientId.Value);

        var projected = query.OrderByDescending(m => m.OpenedDate).Select(m => new MatterDto
        {
            Id = m.Id,
            ReferenceNumber = m.ReferenceNumber,
            ClientId = m.ClientId,
            ClientName = m.Client.Name,
            Title = m.Title,
            Description = m.Description,
            MatterType = m.MatterType,
            Status = m.Status,
            FeeArrangement = m.FeeArrangement,
            EstimatedValue = m.EstimatedValue,
            OpenedDate = m.OpenedDate,
            ClosedDate = m.ClosedDate,
            LeadConsultantId = m.LeadConsultantId,
            LeadConsultantName = m.LeadConsultant.FirstName + " " + m.LeadConsultant.LastName,
            CreatedAt = m.CreatedAt
        });

        var result = await PaginatedList<MatterDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<MatterDto>>.Success(result);
    }
}
