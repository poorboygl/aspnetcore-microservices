namespace EventBus.Messages.IntergrationEvents;

public record IntegrationBaseEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime CreationDate { get; } = DateTime.UtcNow;
}