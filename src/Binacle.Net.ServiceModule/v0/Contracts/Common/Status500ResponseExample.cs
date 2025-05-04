using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class Status500ResponseExample : ISingleOpenApiExamplesProvider<ProblemDetails>
{
	public IOpenApiExample<ProblemDetails> GetExample()
	{
		var ex = new InvalidOperationException("Sample exception message");
		var problemDetails = new ProblemDetails
		{
			Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
			Status = StatusCodes.Status500InternalServerError,
			Title = "Unexpected Server Error",
			Detail = "An unexpected error occurred while processing your request. Please try again later or contact support.",
		};
		
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			problemDetails.Extensions.TryAdd("exception", ex.GetType().Name);
			problemDetails.Extensions.TryAdd("message", ex.Message);
			problemDetails.Extensions.TryAdd("stackTrace", ex.StackTrace);
		}
		
		return OpenApiExample.Create(
			"serverError",
			"Server Error",
			"Example response when something went wrong",
			problemDetails
		);
	}
}