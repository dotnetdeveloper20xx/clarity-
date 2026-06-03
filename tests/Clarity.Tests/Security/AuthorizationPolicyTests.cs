using Clarity.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Security;

public class AuthorizationPolicyTests
{
    [Fact]
    public void RefreshToken_IsActive_WhenNotRevokedAndNotExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_IsNotActive_WhenRevoked()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = true
        };

        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_IsNotActive_WhenExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false
        };

        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_IsExpired_WhenPastExpiryDate()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        };

        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_IsNotExpired_WhenBeforeExpiryDate()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        token.IsExpired.Should().BeFalse();
    }

    [Theory]
    [InlineData(MatterAccessLevel.Read)]
    [InlineData(MatterAccessLevel.Contribute)]
    [InlineData(MatterAccessLevel.Manage)]
    [InlineData(MatterAccessLevel.Restricted)]
    public void MatterAccess_AllLevels_AreValid(MatterAccessLevel level)
    {
        var access = new MatterAccess { AccessLevel = level };
        access.AccessLevel.Should().Be(level);
    }
}
