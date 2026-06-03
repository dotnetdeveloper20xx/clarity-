using Clarity.Application.Common.Interfaces;
using Clarity.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clarity.Application.TimeEntries.Queries.GetTimeEntries;

public class GetTimeEntriesQueryHandler : IRequestHandler<GetTimeEntriesQuery, Result<PaginatedList<TimeEntryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTimeEntriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TimeEntryDto>>> Handle(GetTimeEntriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TimeEntries
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (request.MatterId.HasValue)
            query = query.Where(t => t.MatterId == request.MatterId.Value);
        if (request.UserId.HasValue)
            query = query.Where(t => t.UserId == request.UserId.Value);
        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);
        if (request.FromDate.HasValue)
            query = query.Where(t => t.Date >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(t => t.Date <= request.ToDate.Value);

        var projected = query.OrderByDescending(t => t.Date).Select(t => new TimeEntryDto
        {
            Id = t.Id,
            MatterId = t.MatterId,
            MatterReference = t.Matter.ReferenceNumber,
            MatterTitle = t.Matter.Title,
            UserId = t.UserId,
            UserName = t.User.FirstName + " " + t.User.LastName,
            Date = t.Date,
            DurationMinutes = t.DurationMinutes,
            Description = t.Description,
            IsBillable = t.IsBillable,
            RateAmount = t.RateAmount,
            Status = t.Status,
            CreatedAt = t.CreatedAt
        });

        var result = await PaginatedList<TimeEntryDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
        return Result<PaginatedList<TimeEntryDto>>.Success(result);
    }
}
