namespace Binacle.Net.Api.ServiceModule.v0.Responses;

internal class AuthErrorResponse
{
	public required string Message { get; set; }
	public string[]? Errors { get; set; }
	internal static AuthErrorResponse Create(string message, string[]? errors = null)
	{
		return new AuthErrorResponse
		{
			Message = message,
			Errors = errors
		};
	}
}
