namespace Storygame.Cqrs;

public interface IEventsConsumer
{
    EventsConsumer Register<TEvent>(int msWaitWhenQueueEmpty = 1000) where TEvent : Event;
}