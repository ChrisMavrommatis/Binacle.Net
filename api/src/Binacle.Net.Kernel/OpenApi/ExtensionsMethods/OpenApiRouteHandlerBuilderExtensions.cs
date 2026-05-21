using Binacle.Net.Kernel.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

namespace Binacle.Net;

public static class OpenApiRouteHandlerBuilderExtensions
{
	public static RouteHandlerBuilder ResponseDescription(this RouteHandlerBuilder builder, int statusCode, string description)
	{
		return builder.WithMetadata(new ResponseDescriptionMetadata(statusCode, description));
	}

	public static TBuilder ResponseDescription<TBuilder>(this TBuilder builder, int statusCode, string description)
		where TBuilder : IEndpointConventionBuilder
	{
		return builder.WithMetadata(new ResponseDescriptionMetadata(statusCode, description));
	}
}
