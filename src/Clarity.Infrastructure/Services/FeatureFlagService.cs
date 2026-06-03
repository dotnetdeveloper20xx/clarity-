using Clarity.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Clarity.Infrastructure.Services;

/// <summary>
/// Simple configuration-based feature flags. Can be upgraded to Azure App Configuration or LaunchDarkly.
/// </summary>
public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;

    public FeatureFlagService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled(string featureName)
    {
        var value = _configuration[$"FeatureFlags:{featureName}"];
        return bool.TryParse(value, out var result) && result;
    }
}
