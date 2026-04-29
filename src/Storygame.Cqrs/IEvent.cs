namespace Storygame.Cqrs;

public interface IEvent
{
    Guid EventId { get; }
}
