namespace Clarity.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
