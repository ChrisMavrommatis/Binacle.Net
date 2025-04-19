using Microsoft.Extensions.DependencyInjection;

namespace YetAnotherMediator;

internal abstract class RequestHandlerWrapper<TResponse> 
{
	public abstract ValueTask<TResponse> Handle(
		IRequest<TResponse> request,
		IServiceProvider provider, 
		CancellationToken cancellationToken
	);
}

internal class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
	where TRequest : IRequest<TResponse>
{
	public override ValueTask<TResponse> Handle(IRequest<TResponse> request, IServiceProvider provider, CancellationToken cancellationToken)
	{
		var handler = provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
		return handler.HandleAsync((TRequest)request, cancellationToken);
	}
}

