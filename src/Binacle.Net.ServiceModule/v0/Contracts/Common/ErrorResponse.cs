namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class ErrorResponse
{
	public required string Message { get; set; }
	public string[]? Errors { get; set; }
	internal static ErrorResponse Create(string message, string[]? errors = null)
	{
		return new ErrorResponse
		{
			Message = message,
			Errors = errors
		};
	}
}
