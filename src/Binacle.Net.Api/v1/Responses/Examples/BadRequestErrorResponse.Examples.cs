using Binacle.Net.Api.Constants;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v1.Responses.Examples;

internal class BadRequestErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
{
	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"Validation Error",
			"Validation Error",
			"Example response with validation errors",
			ErrorResponse.Create(ErrorCategory.ValidationError)
				.AddFieldValidationError("Items[0].Height", ErrorMessage.GreaterThanZero)
		);

		yield return OpenApiExample.Create(
			"Request Error",
			"Request Error",
			"Example response when the request is malformed",
			ErrorResponse.Create(ErrorCategory.RequestError)
				.AddParameterError("request", ErrorMessage.MalformedRequestBody)
		);
	}
}
