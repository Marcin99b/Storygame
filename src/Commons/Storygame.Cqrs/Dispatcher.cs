using Gantry.NET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Storygame.Logging;
using Storygame.Storage;

namespace Storygame.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider, IEventsRepository eventsRepository, IGantryClient gantryClient, ILogger<Dispatcher> logger) : IDispatcher
{
    private bool USE_GANTRY = false;

    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        ct.ThrowIfCancellationRequested();
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        logger.ExecutingQuery(typeof(TQuery).Name);

        return handler.HandleAsync(query, ct);
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct)
        where TCommand : ICommand
    {
        ct.ThrowIfCancellationRequested();
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        logger.ExecutingCommand(typeof(TCommand).Name);

        return handler.HandleAsync(command, ct);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : Event
    {
        ct.ThrowIfCancellationRequested();
        logger.PublishingEvent(typeof(TEvent).Name);
        if (!USE_GANTRY)
        {
            await eventsRepository.Publish(@event, ct);
            return;
        }

        try
        {
            var json = JsonConvert.SerializeObject(@event);
            await gantryClient.Put(json);
        }
        finally
        {
            await eventsRepository.Publish(@event, ct);
        }
    }
}
