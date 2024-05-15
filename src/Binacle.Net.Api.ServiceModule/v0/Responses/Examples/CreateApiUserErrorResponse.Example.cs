using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.ServiceModule.v0.Responses.Examples;

internal class CreateApiUserErrorResponseExample : MultipleSwaggerExamplesProvider<ErrorResponse>
{
	public override IEnumerable<ISwaggerExample<ErrorResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("Validation Error", "Validation Error", "Example response with validation errors",
			ErrorResponse.Create("Validation Error",
			[
				"'Email' is not a valid email address.",
				"The length of 'Password' must be at least 10 characters. You entered 8 characters." 
			])
		);

		yield return SwaggerExample.Create("Other Error", "Other Error", "Example response when something went wrong",
			ErrorResponse.Create("Could not create user")
		);
		
	}
}

