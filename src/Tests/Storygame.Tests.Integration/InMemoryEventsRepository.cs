using Storygame.Cqrs;
using Storygame.Storage;

namespace Storygame.Tests.Integration;

internal class InMemoryEventsRepository : IEventsRepository
{
    private readonly Dictionary<string, List<Event>> _events = new();

    private List<Event> GetList<T>() where T : Event
    {
        var key = typeof(T).Name;
        if (!_events.TryGetValue(key, out var list))
        {
            list = new List<Event>();
            _events[key] = list;
        }
        return list;
    }

    public Task Publish<T>(T @event, CancellationToken ct) where T : Event
    {
        GetList<T>().Add(@event);
        return Task.CompletedTask;
    }

    public Task<T> GetById<T>(Guid id, CancellationToken ct) where T : Event
        => Task.FromResult(GetList<T>().OfType<T>().First(e => e.EventId == id));

    public Task<IEnumerable<T>> GetLast<T>(int count, int offset, CancellationToken ct) where T : Event
        => Task.FromResult(GetList<T>().OfType<T>().OrderByDescending(e => e.CreatedAt).Skip(offset).Take(count));

    public Task<T?> Next<T>(Guid previousId, CancellationToken ct) where T : Event
    {
        var ordered = GetList<T>().OfType<T>().OrderBy(e => e.CreatedAt).ToList();
        var index = ordered.FindIndex(e => e.EventId == previousId);
        var next = index >= 0 && index + 1 < ordered.Count ? ordered[index + 1] : null;
        return Task.FromResult(next);
    }

    public Task<T?> GetFirst<T>(CancellationToken ct) where T : Event
        => Task.FromResult(GetList<T>().OfType<T>().OrderBy(e => e.CreatedAt).FirstOrDefault());
}
