namespace Storygame.Cqrs;

public interface IEventsConsumer
{
    Task ConsumeWaitingEvents<TEvent>(CancellationToken ct) where TEvent : Event;
}