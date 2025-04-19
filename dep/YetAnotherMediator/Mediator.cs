using System.Collections.Concurrent;

namespace YetAnotherMediator;

public interface IMediator
{
	public ValueTask<TResponse> SendAsync<TResponse>(
		IRequest<TResponse> request,
		CancellationToken cancellationToken = default
	);

	public ValueTask<TResult> QueryAsync<TResult>(
		IQuery<TResult> query,
		CancellationToken cancellationToken = default
	);

	public ValueTask<TResult> ExecuteAsync<TResult>(
		ICommand<TResult> command,
		CancellationToken cancellationToken = default
	);
}

public class Mediator : IMediator
{
	private readonly ConcurrentDictionary<Type, object> requestHandlersCache = new();
	private readonly ConcurrentDictionary<Type, object> commandHandlersCache = new();
	private readonly ConcurrentDictionary<Type, object> queryHandlersCache = new();
	
	private readonly IServiceProvider serviceProvider;

	public Mediator(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public ValueTask<TResponse> SendAsync<TResponse>(
		IRequest<TResponse> request,
		CancellationToken cancellationToken = default
	)
	{
		var wrapper = (RequestHandlerWrapper<TResponse>)this.requestHandlersCache.GetOrAdd(request.GetType(), static requestType =>
		{
			var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
			return Activator.CreateInstance(wrapperType)!;
		});

		return wrapper.Handle(request, this.serviceProvider, cancellationToken);
	}
	
	public ValueTask<TResult> QueryAsync<TResult>(
		IQuery<TResult> query,
		CancellationToken cancellationToken = default
	)
	{
		var wrapper = (QueryHandlerWrapper<TResult>)this.queryHandlersCache.GetOrAdd(query.GetType(), static queryType =>
		{
			var wrapperType = typeof(QueryHandlerWrapperImpl<,>).MakeGenericType(queryType, typeof(TResult));
			return Activator.CreateInstance(wrapperType)!;
		});
		return wrapper.Handle(query, this.serviceProvider, cancellationToken);
	}
	
	public ValueTask<TResult> ExecuteAsync<TResult>(
		ICommand<TResult> command,
		CancellationToken cancellationToken = default
	)
	{
		var wrapper = (CommandHandlerWrapper<TResult>)this.commandHandlersCache.GetOrAdd(command.GetType(), static commandType =>
		{
			var wrapperType = typeof(CommandHandlerWrapperImpl<,>).MakeGenericType(commandType, typeof(TResult));
			return Activator.CreateInstance(wrapperType)!;
		});
		return wrapper.Handle(command, this.serviceProvider, cancellationToken);
	}
}

