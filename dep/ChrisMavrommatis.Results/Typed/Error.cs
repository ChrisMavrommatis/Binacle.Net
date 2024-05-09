namespace ChrisMavrommatis.Results.Typed;

public struct Error : ITypedResult
{
	public string Message { get; }

	public Error(string message)
	{
		Message = message;
	}
}


