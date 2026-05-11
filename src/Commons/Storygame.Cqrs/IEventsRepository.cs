using Storygame.Cqrs;

namespace Storygame.Storage;

public interface IEventsRepository
{
    Task<T> GetById<T>(Guid id, CancellationToken ct) where T : Event;
    Task<IEnumerable<T>> GetLast<T>(int count, int offset, CancellationToken ct) where T : Event;
    Task<T?> Next<T>(Guid previousId, CancellationToken ct) where T : Event;
    Task<T?> GetFirst<T>(CancellationToken ct) where T : Event;
    Task Publish<T>(T @event, CancellationToken ct) where T : Event;
}
