namespace BluesoftBank.Domain.Interfaces;

public interface IDomainEvent { }

public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
}
