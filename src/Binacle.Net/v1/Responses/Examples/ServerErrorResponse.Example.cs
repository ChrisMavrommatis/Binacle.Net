using Binacle.Net.Constants;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v1.Responses.Examples;

internal class ServerErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"serverError",
			"Server Error",
			ErrorResponse.Create(ErrorCategory.ServerError)
				.AddExceptionError(new InvalidOperationException("Example Exception"))
		);
	}
}
