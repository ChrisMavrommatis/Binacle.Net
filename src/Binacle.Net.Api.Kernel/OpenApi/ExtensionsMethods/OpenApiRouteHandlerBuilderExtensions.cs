using Binacle.Net.Api.Kernel.OpenApi.Models;
using Microsoft.AspNetCore.Builder;

namespace Binacle.Net.Api;

public static class OpenApiRouteHandlerBuilderExtensions
{
	public static RouteHandlerBuilder WithResponseDescription(this RouteHandlerBuilder builder, int statusCode, string description)
	{
		return builder.WithMetadata(new ResponseDescriptionMetadata(statusCode, description));
	}

	public static TBuilder AllEndpointsHaveResponseDescription<TBuilder>(this TBuilder builder, int statusCode, string description)
		where TBuilder : IEndpointConventionBuilder
	{
		return builder.WithMetadata(new ResponseDescriptionMetadata(statusCode, description));
	}
}
