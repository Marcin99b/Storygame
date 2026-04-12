using Microsoft.Extensions.DependencyInjection;

namespace Storygame.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) 
        where TQuery : IQuery<TResult>
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetService<IQueryHandler<TQuery, TResult>>()!;
        return handler.HandleAsync(query);
    }

    public Task SendAsync<TCommand>(TCommand command) 
        where TCommand : ICommand
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetService<ICommandHandler<TCommand>>()!;
        return handler.HandleAsync(command);
    }

    public Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        using var scope = serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>()!;
        var tasks = handlers.Select(x => x.HandleAsync(@event));
        return Task.WhenAll(tasks);
    }
}
