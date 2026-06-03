using Clarity.Application.Clients.Commands.CreateClient;
using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Clarity.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Clarity.Tests.Application.Clients;

public class CreateClientCommandTests
{
    [Fact]
    public void Validator_ShouldFail_WhenNameIsEmpty()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand { Name = "", ClientType = ClientType.Individual };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validator_ShouldFail_WhenNameExceedsMaxLength()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand { Name = new string('A', 201), ClientType = ClientType.Individual };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validator_ShouldFail_WhenEmailIsInvalid()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand { Name = "Test Client", ClientType = ClientType.Individual, Email = "not-an-email" };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validator_ShouldPass_WhenCommandIsValid()
    {
        var validator = new CreateClientCommandValidator();
        var command = new CreateClientCommand
        {
            Name = "Valid Client",
            ClientType = ClientType.Organisation,
            Email = "test@example.com",
            Phone = "0207 123 4567"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
