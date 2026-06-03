using Clarity.Application.Common.Models;
using Clarity.Domain.Enums;
using MediatR;

namespace Clarity.Application.Compliance.Commands.CreateComplianceCheck;

public record CreateComplianceCheckCommand : IRequest<Result<Guid>>
{
    public Guid ClientId { get; init; }
    public Guid? MatterId { get; init; }
    public ComplianceCheckType CheckType { get; init; }
    public ComplianceCheckStatus Status { get; init; } = ComplianceCheckStatus.Pending;
    public RiskLevel? RiskLevel { get; init; }
    public string? Notes { get; init; }
}
