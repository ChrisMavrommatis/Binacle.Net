using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class GetAccountErrorResponseExample : ISingleOpenApiExamplesProvider<ErrorResponse>
{
	public IOpenApiExample<ErrorResponse> GetExample()
	{
		return OpenApiExample.Create(
			"idparametererror",
			"ID Parameter Error",
			ErrorResponse.IdToGuidParameterError
		);
	}
}
