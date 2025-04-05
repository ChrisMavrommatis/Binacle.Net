using Binacle.Net.Api.Constants;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v1.Responses.Examples;

internal class ServerErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"Server Error",
			ErrorResponse.Create(ErrorCategory.ServerError)
				.AddExceptionError(new InvalidOperationException("Example Exception"))
		);
	}
}
