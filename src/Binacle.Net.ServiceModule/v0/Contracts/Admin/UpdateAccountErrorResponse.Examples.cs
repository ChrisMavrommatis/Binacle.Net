using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class UpdateAccountErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
{
	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"malformedRequest",
			"Malformed Request",
			"Example response when the request is has some syntax errors",
			ErrorResponse.MalformedRequest
		);
		yield return OpenApiExample.Create(
			"idparametererror",
			"ID Parameter Error",
			"Example response when you provide and ID that isn't Guid",
			ErrorResponse.IdToGuidParameterError
		);
		yield return OpenApiExample.Create(
			"validationError1",
			"Validation Error 1",
			"Example response with validation errors",
			ErrorResponse.ValidationError(
			[
				"'Email' is not a valid email address.",
				"The length of 'Password' must be at least 10 characters. You entered 8 characters."
			])
		);
			 
		yield return OpenApiExample.Create(
			"validationError2",
			"Validation Error 2",
			"Example response with validation errors",
			ErrorResponse.ValidationError(
			[
				// TODO: Example
			])
		);
	}
}
