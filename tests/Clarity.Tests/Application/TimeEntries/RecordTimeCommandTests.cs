using Clarity.Application.TimeEntries.Commands.RecordTime;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Application.TimeEntries;

public class RecordTimeCommandTests
{
    [Fact]
    public void Validator_ShouldFail_WhenMatterIdIsEmpty()
    {
        var validator = new RecordTimeCommandValidator();
        var command = new RecordTimeCommand
        {
            MatterId = Guid.Empty,
            Date = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 60,
            Description = "Work"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MatterId");
    }

    [Fact]
    public void Validator_ShouldFail_WhenDurationIsZero()
    {
        var validator = new RecordTimeCommandValidator();
        var command = new RecordTimeCommand
        {
            MatterId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 0,
            Description = "Work"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationMinutes");
    }

    [Fact]
    public void Validator_ShouldFail_WhenDurationExceeds24Hours()
    {
        var validator = new RecordTimeCommandValidator();
        var command = new RecordTimeCommand
        {
            MatterId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 1500,
            Description = "Work"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DurationMinutes");
    }

    [Fact]
    public void Validator_ShouldFail_WhenDescriptionIsEmpty()
    {
        var validator = new RecordTimeCommandValidator();
        var command = new RecordTimeCommand
        {
            MatterId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 60,
            Description = ""
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validator_ShouldPass_WhenCommandIsValid()
    {
        var validator = new RecordTimeCommandValidator();
        var command = new RecordTimeCommand
        {
            MatterId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            DurationMinutes = 90,
            Description = "Legal research on contract dispute"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
