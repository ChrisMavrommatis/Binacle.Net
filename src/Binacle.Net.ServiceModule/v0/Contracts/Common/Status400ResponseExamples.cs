using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class Status500ResponseExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"validationError",
			"Validation Error",
			"Example response with validation errors",
			new ProblemDetails()
			
		);

		yield return OpenApiExample.Create(
			"otherError",
			"Other Error",
			"Example response when something went wrong",
			new HttpValidationProblemDetails()
			{
				Status = 400,
				Title = "validation Eror"
			}
		);
	}
}

// internal class InternalServerErrorErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
// {
// 	public IOpenApiExample<ErrorResponse> GetExample()
// 	{
// 		return OpenApiExample.Create(
// 			"internalServerError",
// 			"Internal Server Error",
// 			"Example response when an internal server error occurs",
// 			ErrorResponse.ServerError(new InvalidOperationException("Example Exception"))
// 		);
// 	}
// }

internal class Status400ResponseExamples : IMultipleOpenApiExamplesProvider<ProblemDetails>
{
	public IEnumerable<IOpenApiExample<ProblemDetails>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"validationError",
			"Validation Error",
			"Example response with validation errors",
			new ProblemDetails()
			
		);

		yield return OpenApiExample.Create(
			"otherError",
			"Other Error",
			"Example response when something went wrong",
			new HttpValidationProblemDetails()
			{
				Status = 400,
				Title = "validation Eror"
			}
		);
	}
}
