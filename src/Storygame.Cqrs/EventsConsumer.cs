using Microsoft.Extensions.DependencyInjection;
using Storygame.Storage;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Storygame.Cqrs;

public class EventsConsumer(IServiceProvider serviceProvider, IEventsRepository eventsRepository) : IEventsConsumer, IDisposable
{
    //todo persistent
    private readonly ConcurrentDictionary<string, Guid> lastExecutedEvents = new ConcurrentDictionary<string, Guid>();
    private readonly List<Task> tasks = [];
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public EventsConsumer Register<TEvent>(int msWaitWhenQueueEmpty = 1000) where TEvent : Event
    {
        var eventCacheKey = typeof(TEvent).Name;
        var task = ConsumeWaitingEvents<TEvent>(eventCacheKey, msWaitWhenQueueEmpty, cancellationTokenSource.Token);
        tasks.Add(task);
        return this;
    }

    //todo it needs registration
    private async Task ConsumeWaitingEvents<TEvent>(string eventCacheKey, int msWaitWhenQueueEmpty, CancellationToken ct)
        where TEvent : Event
    {
        while (!ct.IsCancellationRequested)
        {
            await foreach (var @event in GetEventsLoop<TEvent>(eventCacheKey, ct))
            {
                // todo what if cancellation token stopped when some event handlers are executed and saved something to DB?
                // there should be something like transaction or inbox pattern per each handler
                using var scope = serviceProvider.CreateScope();
                var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>()!;
                //todo error handling
                var tasks = handlers.Select(x => x.HandleAsync(@event, ct));
                await Task.WhenAll(tasks);
                lastExecutedEvents.AddOrUpdate(eventCacheKey, _ => @event.EventId, (_, _) => @event.EventId);
            }

            await Task.Delay(msWaitWhenQueueEmpty, ct);
        }
    }

    private async IAsyncEnumerable<TEvent> GetEventsLoop<TEvent>(string eventCacheKey, [EnumeratorCancellation] CancellationToken ct) where TEvent : Event
    {
        var nextEvent = lastExecutedEvents.ContainsKey(eventCacheKey) switch
        {
            true => await eventsRepository.Next<TEvent>(lastExecutedEvents[eventCacheKey], ct),
            false => await eventsRepository.GetFirst<TEvent>(ct)
        };

        if (nextEvent == null)
        {
            yield break;
        }

        yield return nextEvent;
        var previousId = nextEvent.EventId;

        while (!ct.IsCancellationRequested)
        {
            //todo add batch
            var next = await eventsRepository.Next<TEvent>(previousId, ct);
            if (next == null)
            {
                break;
            }

            yield return next;
            previousId = next.EventId;
        }
    }

    public void Dispose()
    {
        if (!cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
            Task.WaitAll(tasks.ToArray());
        }
    }
}
