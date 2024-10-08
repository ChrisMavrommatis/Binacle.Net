using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v1.Responses.Examples;

internal class PresetNotFoundErrorResponseExample : SingleSwaggerExamplesProvider<ErrorResponse>
{
	public override ErrorResponse GetExample()
	{
		return ErrorResponse.Create(Constants.Errors.Categories.PresetDoesntExist);
	}
}
