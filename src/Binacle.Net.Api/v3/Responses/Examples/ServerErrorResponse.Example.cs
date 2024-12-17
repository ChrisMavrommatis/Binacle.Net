using Binacle.Net.Api.v3.Models;
using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v3.Responses.Examples;

internal class ServerErrorResponseExample : SingleSwaggerExamplesProvider<ErrorResponse>
{
	public override ErrorResponse GetExample()
	{
		return Response.ExceptionError(new InvalidOperationException("Example Exception"), Constants.Errors.Categories.ServerError);
	}
}
