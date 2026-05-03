namespace Storygame.Cqrs;

public interface IDispatcher
{
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct) where TQuery : IQuery<TResult>;
    Task SendAsync<TCommand>(TCommand command, CancellationToken ct) where TCommand : ICommand;
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : Event;
}
