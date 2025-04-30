using Microsoft.AspNetCore.Builder;
using OpenApiExamples.Abstractions;
using OpenApiExamples.Models;

namespace OpenApiExamples;

public static class RouteHandlerBuilderExtensions
{
	public static RouteHandlerBuilder RequestExample<T>(
		this RouteHandlerBuilder builder,
		string contentType
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
		string contentType
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
		string contentType
	) where T : class, ISingleOpenApiExamplesProvider
		=> builder.ResponseExample<T>(statusCode.ToString(), contentType);

	public static RouteHandlerBuilder ResponseExample<T>(
		this RouteHandlerBuilder builder,
		string statusCode,
		string contentType
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
		string contentType
	) where T : class, IMultipleOpenApiExamplesProvider
		=> builder.ResponseExamples<T>(statusCode.ToString(), contentType);

	static RouteHandlerBuilder ResponseExamples<T>(
		this RouteHandlerBuilder builder,
		string statusCode,
		string contentType
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
