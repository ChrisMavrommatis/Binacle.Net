using Binacle.Net.ServiceModule.v0.Contracts.Common;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class DeleteAccountErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
{
	public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"idparametererror",
			"ID Parameter Error",
			"Example response when you provide and ID that isn't Guid",
			ErrorResponse.IdToGuidParameterError
		);
	}
}
