using Microsoft.Extensions.DependencyInjection;

namespace YetAnotherMediator;

internal abstract class QueryHandlerWrapper<TResult> 
{
	public abstract ValueTask<TResult> Handle(
		IQuery<TResult> request,
		IServiceProvider provider, 
		CancellationToken cancellationToken
	);
}

internal class QueryHandlerWrapperImpl<TQuery, TResult> : QueryHandlerWrapper<TResult>
	where TQuery : IQuery<TResult>
{
	public override ValueTask<TResult> Handle(IQuery<TResult> query, IServiceProvider provider, CancellationToken cancellationToken)
	{
		var handler = provider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
		return handler.HandleAsync((TQuery)query, cancellationToken);
	}
}
