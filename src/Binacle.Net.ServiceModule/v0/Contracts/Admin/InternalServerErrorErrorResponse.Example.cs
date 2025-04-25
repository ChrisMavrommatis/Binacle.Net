using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class InternalServerErrorErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"internalServerError",
			"Internal Server Error",
			"Example response when an internal server error occurs",
			ErrorResponse.ServerError(new InvalidOperationException("Example Exception"))
		);
	}
}
