using Clarity.Domain.Enums;

namespace Clarity.Application.TimeEntries.Queries.GetTimeEntries;

public record TimeEntryDto
{
    public Guid Id { get; init; }
    public Guid MatterId { get; init; }
    public string MatterReference { get; init; } = string.Empty;
    public string MatterTitle { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public int DurationMinutes { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsBillable { get; init; }
    public decimal? RateAmount { get; init; }
    public TimeEntryStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
