using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storygame.Logging;
using Storygame.Storage;

namespace Storygame.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider, IEventsRepository eventsRepository, ILogger<Dispatcher> logger) : IDispatcher
{
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

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : Event
    {
        ct.ThrowIfCancellationRequested();
        logger.PublishingEvent(typeof(TEvent).Name);
        return eventsRepository.Publish(@event, ct);
    }
}
