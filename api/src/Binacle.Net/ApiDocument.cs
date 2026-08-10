using System.Collections.Generic;
using System.Text;
using Binacle.Net.Kernel.OpenApi;
using Microsoft.OpenApi;

namespace Binacle.Net;


internal static class ApiDocument
{
	internal static void Transform(IOpenApiDocument apiDocument, OpenApiDocument document)
	{
		document.Info.Title = $"Binacle.Net API {apiDocument.Name}";
		document.Info.Version = apiDocument.Version;
		document.Info.Description = __description__
			.Replace("{{status}}", apiDocument.IsExperimental ? __experimentalMessage__: string.Empty)
			.Replace("{{deprecated}}", apiDocument.IsDeprecated ? __deprecatedMessage__: string.Empty);
		document.Info.License = new OpenApiLicense
		{
			Name = "GNU General Public License v3.0",
			Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
		};
		document.Info.Contact = new OpenApiContact
		{
			Name = "Binacle.Net",
			Url = new Uri(Binacle.Net.Metadata.GitHub)
		};

		// Relative "/" because Binacle.Net is self-hosted - the API sits at the root of wherever the reader
		// deployed it, and naming our own demo host would bake it into every generated client as the default.
		// Set rather than left absent, which OpenAPI already reads as "/": generating under a launch profile
		// fills Servers with a localhost URL, and nobody catches that by eye in a published spec.
		document.Servers =
		[
			new OpenApiServer { Url = "/" }
		];

		if (document.Tags is not null)
		{
			foreach (var tag in document.Tags)
			{
				if (tag.Name is not null && __tagDescriptions__.TryGetValue(tag.Name, out var description))
				{
					tag.Description = description;
				}
			}
		}
	}

	private static readonly Dictionary<string, string> __tagDescriptions__ = new(StringComparer.Ordinal)
	{
		["Pack"] = "Pack items into bins and report how they fit.",
		["Fit"] = "Check whether items fit into bins, without computing a full packing.",
		["Presets"] = "Predefined bin sets configured on the server.",
	};
	
	private static string __description__ = new StringBuilder()
		.AppendLine(Binacle.Net.Metadata.Description)
		.AppendLine()
		.AppendLine("{{status}}")
		.AppendLine()
		.AppendLine("{{deprecated}}")
		.AppendLine()
		.AppendLine($"[View on Github]({Binacle.Net.Metadata.GitHub})")
		.AppendLine()
		.AppendLine($"[🐳 Binacle.Net on Dockerhub]({Binacle.Net.Metadata.Dockerhub})")
		.AppendLine()
		.ToString();
	
	private const string __experimentalMessage__ =
		"**Warning: This API version is experimental and may change any time, introducing breaking changes.**";
	
	private const string __deprecatedMessage__ =
		"**This API version has been deprecated and will be removed on the next major version.** <br/>" +
		"**Please use one of the newer APIs available from the explorer.**";
}
