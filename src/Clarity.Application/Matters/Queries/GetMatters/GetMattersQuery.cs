using Clarity.Application.Common.Models;
using Clarity.Application.Matters.Queries.GetMatter;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Matters.Queries.GetMatters;

public record GetMattersQuery : IRequest<Result<PaginatedList<MatterDto>>>
{
    public string? SearchTerm { get; init; }
    public MatterStatus? Status { get; init; }
    public MatterType? MatterType { get; init; }
    public Guid? ClientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
