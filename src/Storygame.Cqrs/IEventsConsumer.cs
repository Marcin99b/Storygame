namespace Storygame.Cqrs;

public interface IEventsConsumer
{
    EventsConsumer Register<TEvent>() where TEvent : Event;
}