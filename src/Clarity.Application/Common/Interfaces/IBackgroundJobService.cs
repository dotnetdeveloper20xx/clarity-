namespace Clarity.Application.Common.Interfaces;

public interface IBackgroundJobService
{
    Task EnqueueAsync(string jobType, object? payload = null, CancellationToken cancellationToken = default);
}
