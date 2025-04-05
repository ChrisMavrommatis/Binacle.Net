using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.ServiceModule.v0.Responses.Examples;

internal class DeleteApiUserErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"Validation Error",
			ErrorResponse.Create("Validation Error",
			[
				"'Email' is not a valid email address."
			])
		); 
	}
}

