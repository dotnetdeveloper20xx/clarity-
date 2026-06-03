using Clarity.Domain.Enums;

namespace Clarity.Application.Workflows.TimeEntryWorkflow;

public static class TimeEntryStatusTransitionValidator
{
    private static readonly Dictionary<TimeEntryStatus, TimeEntryStatus[]> AllowedTransitions = new()
    {
        [TimeEntryStatus.Draft] = [TimeEntryStatus.Submitted],
        [TimeEntryStatus.Submitted] = [TimeEntryStatus.Approved, TimeEntryStatus.Rejected],
        [TimeEntryStatus.Rejected] = [TimeEntryStatus.Draft],
        [TimeEntryStatus.Approved] = [TimeEntryStatus.Billed, TimeEntryStatus.WrittenOff],
        [TimeEntryStatus.Billed] = [], // Terminal
        [TimeEntryStatus.WrittenOff] = [] // Terminal
    };

    public static bool IsValidTransition(TimeEntryStatus from, TimeEntryStatus to)
    {
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
