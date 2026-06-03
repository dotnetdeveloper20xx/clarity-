using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Workflows.MatterWorkflow;

public record ChangeMatterStatusCommand : IRequest<Result>
{
    public Guid MatterId { get; init; }
    public MatterStatus NewStatus { get; init; }
    public string? Reason { get; init; }
}
