using Binacle.Net.Constants;
using Binacle.Net.v3.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v3.Responses.Examples;

internal class ServerErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"server-error",
			Response.ExceptionError(
				new InvalidOperationException("Example Exception"),
				ErrorCategory.ServerError
			)
		);
	}
}
