using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class ServerErrorResponseExample : SingleSwaggerExamplesProvider<ErrorResponse>
{
	public override ErrorResponse GetExample()
	{
		return Responses.Response.ExceptionError(new InvalidOperationException("Example Exception"), Constants.Errors.Categories.ServerError);
	}
}
