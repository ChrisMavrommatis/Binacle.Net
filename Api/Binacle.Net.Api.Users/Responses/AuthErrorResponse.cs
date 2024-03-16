
namespace Binacle.Net.Api.Users.Responses;

internal class AuthErrorResponse
{
	public string Message { get; set; }
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
