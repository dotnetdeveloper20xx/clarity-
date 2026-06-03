using Clarity.Application.Common.Models;
using MediatR;

namespace Clarity.Application.TimeEntries.Commands.RecordTime;

public record RecordTimeCommand : IRequest<Result<Guid>>
{
    public Guid MatterId { get; init; }
    public DateOnly Date { get; init; }
    public int DurationMinutes { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsBillable { get; init; } = true;
    public Guid? BillingRateId { get; init; }
}
