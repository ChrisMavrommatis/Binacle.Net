using Binacle.Net.Constants;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v1.Responses.Examples;

internal class PresetNotFoundErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"presetNotFound",
			"Preset Not Found",
			ErrorResponse.Create(ErrorCategory.PresetDoesntExist)
		);
	}
}
