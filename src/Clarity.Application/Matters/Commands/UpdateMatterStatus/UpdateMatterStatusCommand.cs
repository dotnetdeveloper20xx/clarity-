using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Matters.Commands.UpdateMatterStatus;

public record UpdateMatterStatusCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public MatterStatus NewStatus { get; init; }
    public string? Reason { get; init; }
}
