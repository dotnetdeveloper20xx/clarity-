using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.TimeEntries.Queries.GetTimeEntries;

public record GetTimeEntriesQuery : IRequest<Result<PaginatedList<TimeEntryDto>>>
{
    public Guid? MatterId { get; init; }
    public Guid? UserId { get; init; }
    public TimeEntryStatus? Status { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
