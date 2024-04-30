namespace Binacle.Net.Api.ServiceModule.Data.Models;

internal class Result
{
	private Result()
	{

	}

	public bool Success { get; set; }

	public string Message { get; set; }

	public static Result Successful() => new Result { Success = true };

	public static Result Failed(string message) => new Result { Success = false, Message = message };
}

internal class Result<TValue>
{
	private Result()
	{

	}

	public bool Success { get; set; }

	public string Message { get; set; }

	public TValue? Value { get; set; }

	public static Result<TValue> Successful(TValue value) => new Result<TValue> { Success = true, Value = value };

	public static Result<TValue> Failed(string message) => new Result<TValue> { Success = false, Message = message };
}
