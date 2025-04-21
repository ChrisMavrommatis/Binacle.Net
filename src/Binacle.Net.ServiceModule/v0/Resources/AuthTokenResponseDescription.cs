using System.Text;

namespace Binacle.Net.ServiceModule.v0.Resources;

internal static class AuthTokenResponseDescription
{
	public static readonly string For200OK = new StringBuilder("**OK**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When you have valid credentials.")
		.AppendLine()
		.ToString();

	public static readonly string For401Unauthorized = new StringBuilder("**Unauthorized**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the credentials are invalid.")
		.AppendLine()
		.ToString();

	public static readonly string For403Forbidden = new StringBuilder("**Forbidden**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("When the account status was suspended.")
		.AppendLine()
		.ToString();
}
