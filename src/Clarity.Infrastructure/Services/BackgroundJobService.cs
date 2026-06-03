using System.Text.Json;
using Clarity.Application.Common.Interfaces;
using Clarity.Domain.Entities;
using Clarity.Infrastructure.Persistence;

namespace Clarity.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly ApplicationDbContext _context;

    public BackgroundJobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task EnqueueAsync(string jobType, object? payload = null, CancellationToken cancellationToken = default)
    {
        var job = new BackgroundJob
        {
            JobType = jobType,
            Payload = payload is not null ? JsonSerializer.Serialize(payload) : null,
            Status = BackgroundJobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<BackgroundJob>().Add(job);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
