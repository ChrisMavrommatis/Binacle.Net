namespace YetAnotherMediator;

public interface IRequest<out TResponse>
{
}
public interface IRequestHandler<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
