using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storygame.Logging;

namespace Storygame.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger) : IDispatcher
{
    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) 
        where TQuery : IQuery<TResult>
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        logger.ExecutingQuery(typeof(TQuery).Name);

        return handler.HandleAsync(query);
    }

    public Task SendAsync<TCommand>(TCommand command) 
        where TCommand : ICommand
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        logger.ExecutingCommand(typeof(TCommand).Name);

        return handler.HandleAsync(command);
    }

    public Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        using var scope = serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>()!;

        logger.PublishingEvent(typeof(TEvent).Name);

        var tasks = handlers.Select(x => x.HandleAsync(@event));
        return Task.WhenAll(tasks);
    }
}
