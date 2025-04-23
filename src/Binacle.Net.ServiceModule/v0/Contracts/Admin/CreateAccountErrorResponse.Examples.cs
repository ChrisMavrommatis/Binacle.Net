using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
{
	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"validationError",
			"Validation Error",
			"Example response with validation errors",
			ErrorResponse.ValidationError(
			[
				"'Email' is not a valid email address.",
				"The length of 'Password' must be at least 10 characters. You entered 8 characters."
			])
		);

		yield return OpenApiExample.Create(
			"otherError",
			"Other Error",
			"Example response when something went wrong",
			ErrorResponse.Create("Could not create user")
		);
	}
}
