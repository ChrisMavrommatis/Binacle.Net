using System.Net.Mime;
using Binacle.Net.Kernel.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net;

public static class EndpointsRouteHandlerBuilderExtensions
{
	public static RouteGroupBuilder Produces<TResponse>(
		this RouteGroupBuilder builder,
		int statusCode,
		string? contentType = null,
		params string[] additionalContentTypes
	) 
	{
		return builder.Produces(statusCode, typeof(TResponse), contentType, additionalContentTypes);
	}

	public static RouteGroupBuilder Produces(
		this RouteGroupBuilder builder,
		int statusCode,
		Type? responseType = null,
		string? contentType = null,
		params string[] additionalContentTypes
	)
	{
		if (responseType is not null && string.IsNullOrEmpty(contentType))
		{
			contentType = MediaTypeNames.Application.Json;
		}

		if (contentType is null)
		{
			return builder.WithMetadata(new ProducesResponseTypeMetadata(statusCode, responseType ?? typeof(void)));
		}

		var contentTypes = new string[additionalContentTypes.Length + 1];
		contentTypes[0] = contentType;
		additionalContentTypes.CopyTo(contentTypes, 1);

		return builder.WithMetadata(new ProducesResponseTypeMetadata(statusCode, responseType ?? typeof(void), contentTypes));
	}

	// One marker while there is one core tier. It takes an argument when a second appears.
	public static TBuilder RateLimited<TBuilder>(this TBuilder builder)
		where TBuilder : IEndpointConventionBuilder
	{
		builder.WithMetadata(new RateLimitedMetadata());
		return builder;
	}
}
