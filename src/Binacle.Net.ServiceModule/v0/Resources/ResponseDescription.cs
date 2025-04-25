using System.Text;

namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class ResponseDescription
{
	public static string For400BadRequest = new StringBuilder("**Bad Request**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the request is invalid.")
		.AppendLine()
		.ToString();

	public static string For401Unauthorized = new StringBuilder("**Unauthorized**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When provided user token is invalid.")
		.AppendLine()
		.ToString();

	public static string For403Forbidden = new StringBuilder("**Forbidden**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When provided user token does not have permission.")
		.AppendLine()
		.ToString();
	
	public static string For500InternalServerError = new StringBuilder("**Internal Server Error**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When an internal error occured.")
		.AppendLine()
		.ToString();
}
