using Clarity.Application.Workflows.TimeEntryWorkflow;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Workflows;

public class TimeEntryWorkflowTests
{
    [Theory]
    [InlineData(TimeEntryStatus.Draft, TimeEntryStatus.Submitted, true)]
    [InlineData(TimeEntryStatus.Submitted, TimeEntryStatus.Approved, true)]
    [InlineData(TimeEntryStatus.Submitted, TimeEntryStatus.Rejected, true)]
    [InlineData(TimeEntryStatus.Rejected, TimeEntryStatus.Draft, true)]
    [InlineData(TimeEntryStatus.Approved, TimeEntryStatus.Billed, true)]
    [InlineData(TimeEntryStatus.Approved, TimeEntryStatus.WrittenOff, true)]
    [InlineData(TimeEntryStatus.Draft, TimeEntryStatus.Approved, false)] // Invalid: must submit first
    [InlineData(TimeEntryStatus.Draft, TimeEntryStatus.Billed, false)] // Invalid: can't skip
    [InlineData(TimeEntryStatus.Billed, TimeEntryStatus.Draft, false)] // Invalid: terminal
    [InlineData(TimeEntryStatus.Billed, TimeEntryStatus.Approved, false)] // Invalid: terminal
    [InlineData(TimeEntryStatus.WrittenOff, TimeEntryStatus.Draft, false)] // Invalid: terminal
    public void TimeEntryStatusTransition_ShouldValidateCorrectly(TimeEntryStatus from, TimeEntryStatus to, bool expected)
    {
        var result = TimeEntryStatusTransitionValidator.IsValidTransition(from, to);
        result.Should().Be(expected);
    }
}
