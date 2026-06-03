using Clarity.Application.Workflows.MatterWorkflow;
using Clarity.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Workflows;

public class MatterWorkflowTests
{
    [Theory]
    [InlineData(MatterStatus.Open, MatterStatus.InProgress, true)]
    [InlineData(MatterStatus.Open, MatterStatus.OnHold, true)]
    [InlineData(MatterStatus.Open, MatterStatus.Closed, true)]
    [InlineData(MatterStatus.InProgress, MatterStatus.Closed, true)]
    [InlineData(MatterStatus.OnHold, MatterStatus.Open, true)]
    [InlineData(MatterStatus.AwaitingClient, MatterStatus.InProgress, true)]
    [InlineData(MatterStatus.Closed, MatterStatus.Archived, true)]
    [InlineData(MatterStatus.Open, MatterStatus.Archived, false)] // Invalid: can't skip to Archived
    [InlineData(MatterStatus.Closed, MatterStatus.Open, false)] // Invalid: can't reopen without admin
    [InlineData(MatterStatus.Archived, MatterStatus.Open, false)] // Invalid: terminal
    [InlineData(MatterStatus.OnHold, MatterStatus.Closed, false)] // Invalid: must go through Open first
    public void MatterStatusTransition_ShouldValidateCorrectly(MatterStatus from, MatterStatus to, bool expected)
    {
        var result = MatterStatusTransitionValidator.IsValidTransition(from, to);
        result.Should().Be(expected);
    }

    [Fact]
    public void GetAllowedTransitions_FromOpen_ShouldReturnMultipleOptions()
    {
        var allowed = MatterStatusTransitionValidator.GetAllowedTransitions(MatterStatus.Open);
        allowed.Should().NotBeEmpty();
        allowed.Should().Contain(MatterStatus.InProgress);
        allowed.Should().Contain(MatterStatus.OnHold);
        allowed.Should().Contain(MatterStatus.Closed);
    }

    [Fact]
    public void GetAllowedTransitions_FromArchived_ShouldReturnEmpty()
    {
        var allowed = MatterStatusTransitionValidator.GetAllowedTransitions(MatterStatus.Archived);
        allowed.Should().BeEmpty();
    }
}
