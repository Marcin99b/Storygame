namespace Storygame.Cqrs;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent command);
}