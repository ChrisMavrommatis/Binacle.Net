using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.ServiceModule.Services;

public class RateLimitingResponsesFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		if (context.ApiDescription.RelativePath.Contains("api"))
		{
			operation.Responses.Add(StatusCodes.Status429TooManyRequests.ToString(), new OpenApiResponse 
			{
				Description = @"<b>Too many requests</b>
				<br />
				<p>
					When you have reached the maximum number of requests allowed. Please try again later.
				</p>
				"
			});
		}
	}
}

