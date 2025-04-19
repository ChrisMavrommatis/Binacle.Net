namespace YetAnotherMediator;

public interface ICommand<out TResponse>
{
}

public interface ICommandHandler<in TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	ValueTask<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}



