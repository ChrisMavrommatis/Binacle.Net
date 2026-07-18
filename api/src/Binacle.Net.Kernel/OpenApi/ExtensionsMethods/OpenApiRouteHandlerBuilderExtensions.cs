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

	// Sets the OpenAPI operationId without using WithName, so it stays off the app-wide-unique endpoint name.
	// The operationId becomes the generated SDK method name, so keep it stable once a version ships.
	public static RouteHandlerBuilder WithOperationId(this RouteHandlerBuilder builder, string operationId)
	{
		return builder.WithMetadata(new OperationIdMetadata(operationId));
	}

	public static TBuilder WithOperationId<TBuilder>(this TBuilder builder, string operationId)
		where TBuilder : IEndpointConventionBuilder
	{
		return builder.WithMetadata(new OperationIdMetadata(operationId));
	}
}
