using System.Text;
using Binacle.Net.Kernel.OpenApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.OpenApi.Transformers;

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
		// 429 is documented whenever an endpoint opts into rate limiting, regardless of whether the RateLimiter
		// feature is toggled on right now. It is part of the endpoint's contract, and the published spec (built
		// with the feature off) must still tell a generated client that 429 can happen.
		if (context.Description.ActionDescriptor.EndpointMetadata
		    .OfType<EnableRateLimitingAttribute>()
		    .Any())
		{
			operation.Responses?.Add(
				StatusCode429TooManyRequests,
				OpenApiResponseFor429TooManyRequests
			);
		}

		return Task.CompletedTask;
	}
}
