using System.Text;
using Binacle.Net.Kernel.OpenApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.OpenApi;

internal class RateLimiterResponseOperationTransformer : IOpenApiOperationTransformer
{

	public static readonly OpenApiResponse OpenApiResponseFor429TooManyRequests = new OpenApiResponse
	{
		Description = ResponseDescription.Format(
			StatusCodes.Status429TooManyRequests,
			"You have reached the maximum number of requests allowed. Please try again later."
		)
	};

	public static string StatusCode429TooManyRequests => StatusCodes.Status429TooManyRequests.ToString();
	
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
				StatusCode429TooManyRequests,
				OpenApiResponseFor429TooManyRequests
			);
		}

		return Task.CompletedTask;
	}
}
