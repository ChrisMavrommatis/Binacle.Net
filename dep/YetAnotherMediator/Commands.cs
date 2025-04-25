namespace YetAnotherMediator;

public interface ICommand<out TResponse>
{
}

public interface ICommandHandler;

public interface ICommandHandler<in TCommand, TResult> : ICommandHandler
	where TCommand : ICommand<TResult>
{
	ValueTask<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}



