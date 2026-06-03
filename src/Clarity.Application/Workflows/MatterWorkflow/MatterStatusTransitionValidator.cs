using Clarity.Domain.Enums;

namespace Clarity.Application.Workflows.MatterWorkflow;

/// <summary>
/// Defines valid matter status transitions. Invalid transitions are rejected.
/// </summary>
public static class MatterStatusTransitionValidator
{
    private static readonly Dictionary<MatterStatus, MatterStatus[]> AllowedTransitions = new()
    {
        [MatterStatus.Open] = [MatterStatus.InProgress, MatterStatus.OnHold, MatterStatus.AwaitingClient, MatterStatus.AwaitingThirdParty, MatterStatus.BillingReview, MatterStatus.Closed],
        [MatterStatus.InProgress] = [MatterStatus.OnHold, MatterStatus.AwaitingClient, MatterStatus.AwaitingThirdParty, MatterStatus.BillingReview, MatterStatus.Closed],
        [MatterStatus.OnHold] = [MatterStatus.Open, MatterStatus.InProgress],
        [MatterStatus.AwaitingClient] = [MatterStatus.Open, MatterStatus.InProgress],
        [MatterStatus.AwaitingThirdParty] = [MatterStatus.Open, MatterStatus.InProgress],
        [MatterStatus.BillingReview] = [MatterStatus.InProgress, MatterStatus.Closed],
        [MatterStatus.Closed] = [MatterStatus.Archived], // Reopen requires Admin - handled separately
        [MatterStatus.Archived] = [] // Terminal state
    };

    public static bool IsValidTransition(MatterStatus from, MatterStatus to)
    {
        if (AllowedTransitions.TryGetValue(from, out var allowed))
        {
            return allowed.Contains(to);
        }
        return false;
    }

    public static MatterStatus[] GetAllowedTransitions(MatterStatus current)
    {
        return AllowedTransitions.TryGetValue(current, out var allowed) ? allowed : [];
    }
}
