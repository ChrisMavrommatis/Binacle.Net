namespace YetAnotherMediator.Exceptions;

public class MissingHandlerException : Exception
{
	public string HandlerName { get; }

	public MissingHandlerException(Type type)
		: this(type.FullName ?? type.Name)
	{
	}

	// Constructor that accepts string
	public MissingHandlerException(string handlerName)
		: base($"Handler not found for type: {handlerName}")
	{
		this.HandlerName = handlerName;
	}
}
