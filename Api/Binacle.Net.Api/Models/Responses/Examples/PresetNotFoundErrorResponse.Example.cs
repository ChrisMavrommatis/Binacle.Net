using Binacle.Net.Api.Models.Requests;
using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.Models.Responses.Examples;

public class PresetNotFoundErrorResponseExample : SingleSwaggerExamplesProvider<ErrorResponse>
{
	public override ErrorResponse GetExample()
	{
		return ErrorResponse.Create(Constants.Errors.Categories.ResourceNotFoundError)
			.AddParameterError(nameof(PresetQueryRequestWithBody.Preset), string.Format("preset '{0}' does not exist.", "testxcz"));
	}
}
