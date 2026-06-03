namespace Clarity.Application.Common.Interfaces;

/// <summary>
/// Feature flags allow code to be deployed but hidden. Enable gradually.
/// </summary>
public interface IFeatureFlagService
{
    bool IsEnabled(string featureName);
}
