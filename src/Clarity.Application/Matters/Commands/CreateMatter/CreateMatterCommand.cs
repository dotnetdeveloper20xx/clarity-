using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Matters.Commands.CreateMatter;

public record CreateMatterCommand : IRequest<Result<Guid>>
{
    public Guid ClientId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public MatterType MatterType { get; init; }
    public FeeArrangement FeeArrangement { get; init; }
    public decimal? EstimatedValue { get; init; }
    public Guid LeadConsultantId { get; init; }
}
