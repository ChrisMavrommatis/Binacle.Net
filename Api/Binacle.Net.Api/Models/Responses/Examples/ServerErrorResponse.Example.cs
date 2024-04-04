using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.Models.Responses.Examples;

public class ServerErrorResponseExample : SingleSwaggerExamplesProvider<ErrorResponse>
{
	public override ErrorResponse GetExample()
	{
		return ErrorResponse.Create(Constants.Errors.Categories.ServerError)
			.AddExceptionError(new System.InvalidOperationException("Example Exception"));
	}
}
