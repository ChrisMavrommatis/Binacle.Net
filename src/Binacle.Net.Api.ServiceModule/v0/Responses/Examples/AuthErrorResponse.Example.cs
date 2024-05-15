using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.ServiceModule.v0.Responses.Examples;

internal class AuthErrorResponseExample : MultipleSwaggerExamplesProvider<AuthErrorResponse>
{
	public override IEnumerable<ISwaggerExample<AuthErrorResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("Validation Error", "Validation Error", "Example response with validation errors",
			AuthErrorResponse.Create("Validation Error",
			[
				"'Email' is not a valid email address.",
				"The length of 'Password' must be at least 10 characters. You entered 8 characters."
			])
		);

		yield return SwaggerExample.Create("Other Error", "Other Error", "Example response when something went wrong",
			AuthErrorResponse.Create("Failed to generate token")
		);
	}
}

