namespace YetAnotherMediator;

public interface IQuery<out TResult>
{
}

public interface IQueryHandler<in TQuery, TResult>
	where TQuery : IQuery<TResult>
{
	ValueTask<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
