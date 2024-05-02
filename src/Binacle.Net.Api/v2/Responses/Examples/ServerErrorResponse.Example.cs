using Binacle.Net.Api.v2.Models.Errors;
using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class ServerErrorResponseExample : SingleSwaggerExamplesProvider<Response<List<IApiError>>>
{
	public override Response<List<IApiError>> GetExample()
	{
		return Responses.Response.ExceptionError(new InvalidOperationException("Example Exception"), Constants.Errors.Categories.ServerError);
	}
}
