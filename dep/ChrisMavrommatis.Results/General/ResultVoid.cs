namespace ChrisMavrommatis.Results.General;

public class Result : IGeneralResult
{
	private Result()
	{

	}

	public bool Success { get; set; }

	public string Message { get; set; }

	public static Result Successful() => new Result { Success = true };

	public static Result Failed(string message) => new Result { Success = false, Message = message };
}

