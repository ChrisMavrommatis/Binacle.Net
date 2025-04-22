using Microsoft.Extensions.DependencyInjection;

namespace YetAnotherMediator;

internal abstract class CommandHandlerWrapper<TResult> 
{
	public abstract ValueTask<TResult> Handle(
		ICommand<TResult> request,
		IServiceProvider provider, 
		CancellationToken cancellationToken
	);
}


internal class CommandHandlerWrapperImpl<TCommand, TResult> : CommandHandlerWrapper<TResult>
	where TCommand : ICommand<TResult>
{
	public override ValueTask<TResult> Handle(ICommand<TResult> command, IServiceProvider provider, CancellationToken cancellationToken)
	{
		var handler = provider.GetService<ICommandHandler<TCommand, TResult>>();
		// var handler = provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
		return handler.HandleAsync((TCommand)command, cancellationToken);
	}
}
