using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Kernel.OpenApi.Helpers;

public static class OpenApiValidationProblemExample
{
	public static IOpenApiExample<ProblemDetails> Create(IDictionary<string, string[]> errors)
	{
		return OpenApiExample.Create(
			"validationProblem",
			"Validation Problem",
			new HttpValidationProblemDetails(errors)
			{
				Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
				Status = StatusCodes.Status422UnprocessableEntity
			}
		);
	}
	
	public static IOpenApiExample<ProblemDetails> Create(
		string key, 
		IDictionary<string, string[]> errors)
	{
		return OpenApiExample.Create(
			key,
			new HttpValidationProblemDetails(errors)
			{
				Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
				Status = StatusCodes.Status422UnprocessableEntity
			}
		);
	}

	public static IOpenApiExample<ProblemDetails> Create(
		string key, 
		string summary,
		IDictionary<string, string[]> errors)
	{
		return OpenApiExample.Create(
			key,
			summary,
			new HttpValidationProblemDetails(errors)
			{
				Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
				Status = StatusCodes.Status422UnprocessableEntity
			}
		);
	}


	public static IOpenApiExample<ProblemDetails> Create(
		string key,
		string summary, 
		string description,
		IDictionary<string, string[]> errors)
	{
		return OpenApiExample.Create(
			key,
			summary,
			description,
			new HttpValidationProblemDetails(errors)
			{
				Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
				Status = StatusCodes.Status422UnprocessableEntity
			}
		);
	}
}
