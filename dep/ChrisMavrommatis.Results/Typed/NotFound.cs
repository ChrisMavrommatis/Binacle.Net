namespace ChrisMavrommatis.Results.Typed;

public struct NotFound : ITypedResult
{
	public string Message { get; }

	public NotFound(string message)
	{
		Message = message;
	}
}


