using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class AuthErrorResponseExamples : IMultipleOpenApiExamplesProvider<AuthErrorResponse>
{
	public IEnumerable<IOpenApiExample<AuthErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"validationError",
			"Validation Error",
			"Example response with validation errors",
			AuthErrorResponse.Create("Validation Error",
			[
				"'Email' is not a valid email address.",
				"The length of 'Password' must be at least 10 characters. You entered 8 characters."
			])
		);

		yield return OpenApiExample.Create(
			"otherError",
			"Other Error",
			"Example response when something went wrong",
			AuthErrorResponse.Create("Failed to generate token")
		);
	}
}

