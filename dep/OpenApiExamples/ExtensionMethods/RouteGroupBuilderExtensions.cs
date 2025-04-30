using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples.Abstractions;
using OpenApiExamples.Models;

namespace OpenApiExamples;

public static class RouteGroupBuilderExtensions
{

	public static RouteGroupBuilder ResponseExample<T>(
		this RouteGroupBuilder builder,
		int statusCode,
		string contentType
	) where T : class, ISingleOpenApiExamplesProvider
		=> builder.ResponseExample<T>(statusCode.ToString(), contentType);

	public static RouteGroupBuilder ResponseExample<T>(
		this RouteGroupBuilder builder,
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
	
	
	public static RouteGroupBuilder ResponseExamples<T>(
		this RouteGroupBuilder builder,
		int statusCode,
		string contentType
	) where T : class, IMultipleOpenApiExamplesProvider
		=> builder.ResponseExamples<T>(statusCode.ToString(), contentType);

	static RouteGroupBuilder ResponseExamples<T>(
		this RouteGroupBuilder builder,
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
