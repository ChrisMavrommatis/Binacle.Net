using System.Net;
using System.Text;
using Binacle.Net.Kernel.OpenApi.Models;

namespace Binacle.Net.Kernel.OpenApi.Helpers;

public static class ResponseDescription
{
	public static string Format(int statusCode, string description)
	{
		var statusDescription = HttpStatusDescriptions.For(statusCode);
		return new StringBuilder($"**{statusDescription}**")
			.AppendLine("<br />")
			.AppendLine()
			.AppendLine(description)
			.AppendLine()
			.ToString();
	}

	internal static string Format(ResponseDescriptionMetadata metadata)
		=> Format(metadata.StatusCode, metadata.Description);

	public static string Format(HttpStatusCode statusCode, string description)
		=> Format((int)statusCode, description);
}
