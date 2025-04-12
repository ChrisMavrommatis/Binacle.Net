﻿using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.OpenApi;

internal class RateLimiterResponseOperationTransformer : IOpenApiOperationTransformer
{
	public static string For429TooManyRequests =new StringBuilder("**Too Many Requests**")
		.AppendLine("<br />")
		.AppendLine()
		.AppendLine("You have reached the maximum number of requests allowed. Please try again later.")
		.AppendLine()
		.ToString();
	
	public Task TransformAsync(
		OpenApiOperation operation,
		OpenApiOperationTransformerContext context,
		CancellationToken cancellationToken
	)
	{
		// if rate limiting is enabled
		var options = context.ApplicationServices.GetService<IOptions<FeatureOptions>>();
		
		if (!(options?.Value.IsFeatureEnabled("RateLimiter") ?? false))
		{
			return Task.CompletedTask;
		}
		
		
		if (context.Description.ActionDescriptor.EndpointMetadata
		    .OfType<EnableRateLimitingAttribute>()
		    .Any())
		{
			operation.Responses.Add(
				StatusCodes.Status429TooManyRequests.ToString(),
				new OpenApiResponse
				{
					Description = For429TooManyRequests
				}
			);
		}

		return Task.CompletedTask;
	}
}
