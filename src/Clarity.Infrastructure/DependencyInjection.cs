using Clarity.Application.Common.Interfaces;
using Clarity.Infrastructure.Persistence;
using Clarity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clarity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=DESKTOP-VVJN96B;Database=ClarityDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IFileStorageService>(_ => new FileStorageService());
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IActivityTimelineService, ActivityTimelineService>();
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();
        services.AddSingleton<IFeatureFlagService, FeatureFlagService>();

        return services;
    }
}
