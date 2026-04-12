namespace Storygame.Cqrs;

public interface IQueryHandler<TQuery,TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}
