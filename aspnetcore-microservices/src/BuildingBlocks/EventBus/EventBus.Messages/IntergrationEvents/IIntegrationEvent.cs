namespace EventBus.Messages.IntergrationEvents;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationDate { get; }
}