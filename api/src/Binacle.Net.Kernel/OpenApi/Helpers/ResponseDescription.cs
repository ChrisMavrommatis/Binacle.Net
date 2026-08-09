using System.Net;
using Binacle.Net.Kernel.OpenApi.Models;

namespace Binacle.Net.Kernel.OpenApi.Helpers;

public static class ResponseDescription
{
	// Plain prose only: this string is the OpenAPI response description, which SDK generators dump verbatim into
	// generated code (exception messages, doc comments). Markdown/HTML here (**bold**, <br />) renders in Swagger
	// UI but becomes noise in every generated client, so keep it clean.
	public static string Format(int statusCode, string description)
	{
		var statusDescription = HttpStatusDescriptions.For(statusCode);
		return $"{statusDescription}. {description}";
	}

	internal static string Format(ResponseDescriptionMetadata metadata)
		=> Format(metadata.StatusCode, metadata.Description);

	public static string Format(HttpStatusCode statusCode, string description)
		=> Format((int)statusCode, description);
}
