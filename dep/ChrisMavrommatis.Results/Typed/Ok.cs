namespace ChrisMavrommatis.Results.Typed;

public struct Ok : ITypedResult
{
	public string Message { get; }

	public Ok(string message)
	{
		Message = message;
	}
}


