namespace Storygame.Cqrs;

public interface IEventHandler<TEvent> where TEvent : Event
{
    Task HandleAsync(TEvent command, CancellationToken ct);
}