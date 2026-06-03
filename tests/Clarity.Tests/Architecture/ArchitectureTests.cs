using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Architecture;

/// <summary>
/// Architecture tests protect the Clean Architecture dependency rules.
/// These prevent architectural decay over time.
/// </summary>
public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(Clarity.Domain.Common.BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Clarity.Application.DependencyInjection).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Clarity.Infrastructure.DependencyInjection).Assembly;

    [Fact]
    public void Domain_ShouldNotReference_Application()
    {
        var references = DomainAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name == "Clarity.Application",
            "Domain must not depend on Application layer");
    }

    [Fact]
    public void Domain_ShouldNotReference_Infrastructure()
    {
        var references = DomainAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name == "Clarity.Infrastructure",
            "Domain must not depend on Infrastructure layer");
    }

    [Fact]
    public void Domain_ShouldNotReference_Api()
    {
        var references = DomainAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name == "Clarity.Api",
            "Domain must not depend on Api layer");
    }

    [Fact]
    public void Application_ShouldNotReference_Infrastructure()
    {
        var references = ApplicationAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name == "Clarity.Infrastructure",
            "Application must not depend on Infrastructure layer");
    }

    [Fact]
    public void Application_ShouldNotReference_Api()
    {
        var references = ApplicationAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name == "Clarity.Api",
            "Application must not depend on Api layer");
    }

    [Fact]
    public void Domain_ShouldNotReference_EntityFramework()
    {
        var references = DomainAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name != null && r.Name.Contains("EntityFrameworkCore"),
            "Domain must not depend on Entity Framework");
    }

    [Fact]
    public void Domain_ShouldNotReference_AspNetCore()
    {
        var references = DomainAssembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name != null && r.Name.Contains("AspNetCore"),
            "Domain must not depend on ASP.NET Core");
    }

    [Fact]
    public void AllEntities_ShouldBeInDomainProject()
    {
        var domainTypes = DomainAssembly.GetTypes()
            .Where(t => t.Namespace != null && t.Namespace.Contains("Entities"))
            .ToList();

        domainTypes.Should().NotBeEmpty("Domain should contain entity classes");
    }

    [Fact]
    public void AllCommandHandlers_ShouldBeInApplicationProject()
    {
        var handlers = ApplicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("CommandHandler") || t.Name.EndsWith("QueryHandler"))
            .ToList();

        handlers.Should().NotBeEmpty("Application should contain command/query handlers");
    }

    [Fact]
    public void Infrastructure_ShouldReference_Application()
    {
        var references = InfrastructureAssembly.GetReferencedAssemblies();
        references.Should().Contain(r => r.Name == "Clarity.Application",
            "Infrastructure should reference Application to implement its interfaces");
    }

    [Fact]
    public void Infrastructure_ShouldReference_Domain()
    {
        var references = InfrastructureAssembly.GetReferencedAssemblies();
        references.Should().Contain(r => r.Name == "Clarity.Domain",
            "Infrastructure should reference Domain for entity types");
    }
}
