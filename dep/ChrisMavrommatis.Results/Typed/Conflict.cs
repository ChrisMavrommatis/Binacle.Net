namespace ChrisMavrommatis.Results.Typed;

public struct Conflict : ITypedResult
{
	public string Message { get; }

	public Conflict(string message)
	{
		Message = message;
	}
}


