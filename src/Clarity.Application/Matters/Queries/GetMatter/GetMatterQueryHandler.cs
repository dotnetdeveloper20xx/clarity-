using Clarity.Application.Common.Exceptions;
using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using Clarity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.Matters.Queries.GetMatter;

public class GetMatterQueryHandler : IRequestHandler<GetMatterQuery, Result<MatterDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMatterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MatterDto>> Handle(GetMatterQuery request, CancellationToken cancellationToken)
    {
        var matter = await _context.Matters
            .AsNoTracking()
            .Where(m => m.Id == request.Id && !m.IsDeleted)
            .Select(m => new MatterDto
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
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (matter is null)
            throw new NotFoundException(nameof(Matter), request.Id);

        return Result<MatterDto>.Success(matter);
    }
}
