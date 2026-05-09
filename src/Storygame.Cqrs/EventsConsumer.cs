using Microsoft.Extensions.DependencyInjection;
using Storygame.Storage;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Storygame.Cqrs;

public class EventsConsumer(IServiceProvider serviceProvider, IEventsRepository eventsRepository) : IEventsConsumer
{
    //todo persistent
    private readonly ConcurrentDictionary<string, Guid> lastExecutedEvents = new ConcurrentDictionary<string, Guid>();

    //todo it needs registration
    public async Task ConsumeWaitingEvents<TEvent>(CancellationToken ct)
        where TEvent : Event
    {
        var name = typeof(TEvent).FullName!;
        while (!ct.IsCancellationRequested)
        {
            var nextEvent = lastExecutedEvents.ContainsKey(name) switch
            {
                true => await eventsRepository.Next<TEvent>(lastExecutedEvents[name], ct),
                false => await eventsRepository.GetFirst<TEvent>(ct)
            };

            if (nextEvent == null)
            {
                await Task.Delay(1000, ct);
                continue;
            }

            await foreach (var @event in GetEventsLoop<TEvent>(nextEvent, ct))
            {
                // todo what if cancellation token stopped when some event handlers are executed and saved something to DB?
                // there should be something like transaction or inbox pattern per each handler
                using var scope = serviceProvider.CreateScope();
                var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>()!;
                var tasks = handlers.Select(x => x.HandleAsync(@event, ct));
                await Task.WhenAll(tasks);
                lastExecutedEvents.AddOrUpdate(name, _ => @event.EventId, (_, _) => @event.EventId);
            }

            await Task.Delay(1000, ct);
        }
    }

    private async IAsyncEnumerable<TEvent> GetEventsLoop<TEvent>(TEvent firstToExecute, [EnumeratorCancellation] CancellationToken ct) where TEvent : Event
    {
        yield return firstToExecute;
        var previousId = firstToExecute.EventId;

        while (!ct.IsCancellationRequested)
        {
            var next = await eventsRepository.Next<TEvent>(previousId, ct);
            if (next == null)
            {
                break;
            }

            yield return next;
            previousId = next.EventId;
        }
    }
}