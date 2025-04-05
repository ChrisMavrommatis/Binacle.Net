using Binacle.Net.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples.Abstractions;
using OpenApiExamples.Models;

namespace OpenApiExamples;

public static class RouteHandlerBuilderExtensions
{
	public static RouteHandlerBuilder RequestExample<T>(
		this RouteHandlerBuilder builder,
		string contentType = "application/json"
	)
		where T : class, ISingleOpenApiExamplesProvider
	{
		return builder.WithMetadata(
			new RequestExampleMetadata(
				contentType,
				typeof(T)
			)
		);
	}

	public static RouteHandlerBuilder RequestExamples<T>(
		this RouteHandlerBuilder builder,
		string contentType = "application/json"
	)
		where T : class, IMultipleOpenApiExamplesProvider
	{
		return builder.WithMetadata(
			new RequestExampleMetadata(
				contentType,
				typeof(T)
			)
		);
	}

	public static RouteHandlerBuilder ResponseExample<T>(
		this RouteHandlerBuilder builder,
		int statusCode,
		string contentType = "application/json"
	) where T : class, ISingleOpenApiExamplesProvider
		=> builder.ResponseExample<T>(statusCode.ToString(), contentType);

	public static RouteHandlerBuilder ResponseExample<T>(
		this RouteHandlerBuilder builder,
		string statusCode,
		string contentType = "application/json"
	) where T : class, ISingleOpenApiExamplesProvider
	{
		return builder.WithMetadata(
			new ResponseExampleMetadata(
				statusCode,
				contentType,
				typeof(T)
			)
		);
	}

	public static RouteHandlerBuilder ResponseExamples<T>(
		this RouteHandlerBuilder builder,
		int statusCode,
		string contentType = "application/json"
	) where T : class, IMultipleOpenApiExamplesProvider
		=> builder.ResponseExamples<T>(statusCode.ToString(), contentType);

	static RouteHandlerBuilder ResponseExamples<T>(
		this RouteHandlerBuilder builder,
		string statusCode,
		string contentType = "application/json"
	) where T : class, IMultipleOpenApiExamplesProvider
	{
		return builder.WithMetadata(
			new ResponseExampleMetadata(
				statusCode,
				contentType,
				typeof(T)
			)
		);
	}
	
	
	
	public static RouteGroupBuilder AllEndpointsHaveResponseExample<T>(
		this RouteGroupBuilder builder,
		int statusCode,
		string contentType = "application/json"
	) where T : class, ISingleOpenApiExamplesProvider
		=> builder.AllEndpointsHaveResponseExample<T>(statusCode.ToString(), contentType);

	public static RouteGroupBuilder AllEndpointsHaveResponseExample<T>(
		this RouteGroupBuilder builder,
		string statusCode,
		string contentType = "application/json"
	) where T : class, ISingleOpenApiExamplesProvider
	{
		return builder.WithMetadata(
			new ResponseExampleMetadata(
				statusCode,
				contentType,
				typeof(T)
			)
		);
	}
	
	
	public static RouteGroupBuilder AllEndpointsHaveResponseExamples<T>(
		this RouteGroupBuilder builder,
		int statusCode,
		string contentType = "application/json"
	) where T : class, IMultipleOpenApiExamplesProvider
		=> builder.AllEndpointsHaveResponseExamples<T>(statusCode.ToString(), contentType);

	static RouteGroupBuilder AllEndpointsHaveResponseExamples<T>(
		this RouteGroupBuilder builder,
		string statusCode,
		string contentType = "application/json"
	) where T : class, IMultipleOpenApiExamplesProvider
	{
		return builder.WithMetadata(
			new ResponseExampleMetadata(
				statusCode,
				contentType,
				typeof(T)
			)
		);
	}
	
	
}
