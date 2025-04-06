using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Responses.Examples;

internal class ChangeApiUserPasswordErrorResponseExample : IMultipleOpenApiExamplesProvider<ErrorResponse>
{
	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create("Validation Error", "Validation Error", "Example response with validation errors",
			ErrorResponse.Create("Validation Error",
			[
				"'Email' is not a valid email address.",
				"The length of 'Password' must be at least 10 characters. You entered 8 characters." 
			])
		);

		yield return OpenApiExample.Create("Other Error", "Other Error", "Example response when something went wrong",
			ErrorResponse.Create("Could not change user's password")
		);
		
	}
}

