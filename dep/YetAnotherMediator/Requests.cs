namespace YetAnotherMediator;

public interface IRequest<out TResponse>
{
}

public interface IRequestHandler;

public interface IRequestHandler<in TRequest, TResponse> : IRequestHandler
	where TRequest : IRequest<TResponse>
{
	ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
