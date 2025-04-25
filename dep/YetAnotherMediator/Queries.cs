namespace YetAnotherMediator;

public interface IQuery<out TResult>
{
}

public interface IQueryHandler;

public interface IQueryHandler<in TQuery, TResult> : IQueryHandler
	where TQuery : IQuery<TResult>
{
	ValueTask<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
