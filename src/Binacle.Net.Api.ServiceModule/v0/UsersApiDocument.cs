﻿using Microsoft.OpenApi.Models;

namespace Binacle.Net.Api.ServiceModule.v0;

internal static class UsersApiDocument
{
	internal const string ApiName = "users";

	internal const string Endpoint = $"/swagger/{ApiName}/swagger.json";
	internal const string EndpointName = $"Binacle.Net Users API";

	internal static OpenApiInfo ApiInfo = new OpenApiInfo()
	{
		Title = "Binacle.Net Users API",
		Version = "1.0",
		Description = __description__,
		// gpl 3 license
		License = new OpenApiLicense
		{
			Name = "GNU General Public License v3.0",
			Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.html")
		}
	};

	private const string __description__ = $"""
		**Binacle.Net Users API for User Management**
		
		This section is designed only for when Binacle.Net is used as public service. <br>
		User Management is done only by a user of Admin Group.
		
		[View on Github]({Binacle.Net.Api.Metadata.GitHub})

		[🐳 Binacle.Net on Dockerhub]({Binacle.Net.Api.Metadata.Dockerhub})

		[Get Postman collection]({Binacle.Net.Api.Metadata.Postman})

		""";
}

