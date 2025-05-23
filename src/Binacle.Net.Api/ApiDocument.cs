﻿using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace Binacle.Net.Api;

internal static class ApiDocument
{
	internal static OpenApiInfo CreateApiInfo(IApiVersion apiVersion, ApiVersionDescription description)
	{
		var info = new OpenApiInfo()
		{
			Title = $"Binacle.Net API",
			Version = $"{description.ApiVersion.ToString()}",
			Description = __description__,
			// gpl 3 license
			License = new OpenApiLicense
			{
				Name = "GNU General Public License v3.0",
				Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
			},
		};

		info.Description = info.Description
			.Replace("{{status}}", apiVersion.Experimental ? __experimentalMessage__: string.Empty)
			.Replace("{{deprecated}}", apiVersion.Deprecated ? __deprecatedMessage__: string.Empty);

		return info;
	}
	
	private static string __description__ = new StringBuilder()
		.AppendLine(Binacle.Net.Api.Metadata.Description)
		.AppendLine()
		.AppendLine("{{status}}")
		.AppendLine()
		.AppendLine("{{deprecated}}")
		.AppendLine()
		.AppendLine($"[View on Github]({Binacle.Net.Api.Metadata.GitHub})")
		.AppendLine()
		.AppendLine($"[🐳 Binacle.Net on Dockerhub]({Binacle.Net.Api.Metadata.Dockerhub})")
		.AppendLine()
		.AppendLine($"[Get Postman collection]({Binacle.Net.Api.Metadata.Postman})")
		.AppendLine()
		.ToString();
	
	private const string __experimentalMessage__ =
		"**Warning: This API version is experimental and may change any time, introducing breaking changes.**";
	
	private const string __deprecatedMessage__ =
		"**This API version has been deprecated and will be removed on the next major version.** <br/>" +
		"**Please use one of the newer APIs available from the explorer.**";
}
