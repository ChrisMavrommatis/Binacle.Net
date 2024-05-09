namespace ChrisMavrommatis.Results.Typed;

public struct Unauthorized : ITypedResult
{
	public string Message { get; }

	public Unauthorized(string message)
	{
		Message = message;
	}
}


