// using Binacle.Net.Api.ServiceModule.Configuration.Models;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Options;
// using Microsoft.OpenApi.Models;
// using Swashbuckle.AspNetCore.SwaggerGen;
//
// namespace Binacle.Net.Api.ServiceModule.Services;
//
// internal class RateLimitingResponsesFilter : IOperationFilter
// {
// 	private readonly IOptions<RateLimiterConfigurationOptions> options;
//
// 	public RateLimitingResponsesFilter(
// 		IOptions<RateLimiterConfigurationOptions> options
// 	)
// 	{
// 		this.options = options;
// 	}
// 	public void Apply(OpenApiOperation operation, OperationFilterContext context)
// 	{
// 		if (ModuleConstants.RateLimitedPaths.Any(url => context.ApiDescription.RelativePath?.StartsWith(url.TrimStart('/')) ?? false))
// 		{
// 			operation.Responses.Add(StatusCodes.Status429TooManyRequests.ToString(), new OpenApiResponse
// 			{
// 				Description = @"<b>Too many requests</b>
// 				<br />
// 				<p>
// 					When you have reached the maximum number of requests allowed. Please try again later.
// 				</p>
// 				"
// 			});
// 		}
//
//
// 	}
// }
//
