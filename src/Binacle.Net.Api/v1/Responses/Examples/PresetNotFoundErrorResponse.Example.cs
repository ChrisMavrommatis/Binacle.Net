using Binacle.Net.Api.Constants;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v1.Responses.Examples;

internal class PresetNotFoundErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"Preset Not Found",
			ErrorResponse.Create(ErrorCategory.PresetDoesntExist)
		);
	}
}
