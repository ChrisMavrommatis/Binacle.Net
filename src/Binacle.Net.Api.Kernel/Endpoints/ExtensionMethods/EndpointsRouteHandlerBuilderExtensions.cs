﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Binacle.Net.Api;

public static class EndpointsRouteHandlerBuilderExtensions
{
	public static RouteGroupBuilder AllEndpointsProduce<TResponse>(
		this RouteGroupBuilder builder,
		int statusCode,
		string? contentType = null,
		params string[] additionalContentTypes
	) 
	{
		return builder.AllEndpointsProduce(statusCode, typeof(TResponse), contentType, additionalContentTypes);
	}

	public static RouteGroupBuilder AllEndpointsProduce(
		this RouteGroupBuilder builder,
		int statusCode,
		Type? responseType = null,
		string? contentType = null,
		params string[] additionalContentTypes
	)
	{
		if (responseType is Type && string.IsNullOrEmpty(contentType))
		{
			contentType = "application/json";
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
}
