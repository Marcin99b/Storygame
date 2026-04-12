namespace Storygame.Cqrs;

public interface IDispatcher
{
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
}
