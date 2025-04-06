using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Responses.Examples;

internal class UpdateApiUserErrorResponseExample : IMultipleOpenApiExamplesProvider<ErrorResponse>
{
	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"Validation Error 1",
			"Validation Error 1",
			ErrorResponse.Create("Validation Error",
			[
				"'Email' is not a valid email address.",
			])
		);

		yield return OpenApiExample.Create(
			"Validation Error 2",
			"Validation Error 2",
			ErrorResponse.Create("Validation Error",
			[
				"'Type' must not be empty when 'Status' is empty and only accepts the following values: 'User', 'Admin'",
				"'Status' must not be empty when 'Type' is empty and only accepts the following values: 'Active', 'Inactive'",
			])
		);
	}
}
