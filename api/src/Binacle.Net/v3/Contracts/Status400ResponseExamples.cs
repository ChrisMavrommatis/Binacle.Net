using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591
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
				Detail= "'p' is an invalid start of a property name. Expected a '\"'. Path: $ | LineNumber: 1 | BytePositionInLine: 2."
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
