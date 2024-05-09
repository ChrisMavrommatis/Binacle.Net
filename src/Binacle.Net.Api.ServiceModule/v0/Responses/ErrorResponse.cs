namespace Binacle.Net.Api.ServiceModule.v0.Responses;

internal class ErrorResponse
{
	public string Message { get; set; }
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
