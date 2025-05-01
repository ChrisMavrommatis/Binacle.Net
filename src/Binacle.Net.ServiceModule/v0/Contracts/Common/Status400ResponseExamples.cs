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

internal class Status400ResponseExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		
		yield return OpenApiExample.Create(
			"invalidJson",
			"Invalid JSON",
			"Example response when JSON is invalid",
			new ProblemDetails()
			{
				Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
				Status = StatusCodes.Status400BadRequest,
				Title = "Invalid JSON Format",
				Detail= "'s' is an invalid start of a value. Path: $.Password | LineNumber: 3 | BytePositionInLine: 14."
			}
		);
		
		yield return OpenApiExample.Create(
			"malformedRequest",
			"Malformed Request",
			"Example response when the server could not read the request",
			new ProblemDetails()
			{
				Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
				Status = StatusCodes.Status400BadRequest,
				Title = "Malformed Request",
				Detail= "The server could not process the request because it is malformed or contains invalid data. Please verify the request format and try again."
			}
		);
	}
}
