using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storygame.Logging;

namespace Storygame.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger) : IDispatcher
{
    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        ct.ThrowIfCancellationRequested();
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        logger.ExecutingQuery(typeof(TQuery).Name);

        return handler.HandleAsync(query, ct);
    }

    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct)
        where TCommand : ICommand
    {
        ct.ThrowIfCancellationRequested();
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        logger.ExecutingCommand(typeof(TCommand).Name);

        return handler.HandleAsync(command, ct);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : Event
    {
        ct.ThrowIfCancellationRequested();
        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>()!;

        logger.PublishingEvent(typeof(TEvent).Name);
        // todo what if cancellation token stopped when some event handlers are executed and saved something to DB?
        // there should be something like transaction or inbox pattern
        var tasks = handlers.Select(x => x.HandleAsync(@event, ct));
        return Task.WhenAll(tasks);
    }
}
