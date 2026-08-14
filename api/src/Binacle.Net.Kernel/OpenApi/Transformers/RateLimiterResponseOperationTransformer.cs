using Binacle.Net.Kernel.OpenApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;

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
		// The metadata is the whole guard. Only the module that supplies a limiter attaches it, off the core's
		// policy-neutral .RateLimited() marker, so it cannot be here in a build that limits nobody.
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
