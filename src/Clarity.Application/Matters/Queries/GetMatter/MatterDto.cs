using Clarity.Domain.Enums;

namespace Clarity.Application.Matters.Queries.GetMatter;

public record MatterDto
{
    public Guid Id { get; init; }
    public string ReferenceNumber { get; init; } = string.Empty;
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public MatterType MatterType { get; init; }
    public MatterStatus Status { get; init; }
    public FeeArrangement FeeArrangement { get; init; }
    public decimal? EstimatedValue { get; init; }
    public DateOnly OpenedDate { get; init; }
    public DateOnly? ClosedDate { get; init; }
    public Guid LeadConsultantId { get; init; }
    public string LeadConsultantName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
