using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.ServiceModule.v0.Responses.Examples;

internal class DeleteApiUserErrorResponseExample : SingleSwaggerExamplesProvider<ErrorResponse>
{
	public override ErrorResponse GetExample()
	{
		return ErrorResponse.Create("Validation Error",
		[
			"'Email' is not a valid email address."
		]);
	}
}

