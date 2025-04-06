using Binacle.Net.v1.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v1.Responses.Examples;

internal class PresetQueryResponseExamples : IMultipleOpenApiExamplesProvider<QueryResponse>
{
	public IEnumerable<IOpenApiExample<QueryResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"Success",
			"Found Bin",
			"Response example when a bin is found",
			new QueryResponse
			{
				Result = ResultType.Success,
				Bin = new() { ID = "Small", Length = 10, Width = 40, Height = 60 },
			}
		);

		yield return OpenApiExample.Create(
			"Failure",
			"Bin not Found",
			"Response example when a bin is not found",
			new QueryResponse
			{
				Result = ResultType.Failure,
				Message = "Failed to find bin. Reason: ItemDimensionExceeded"
			}
		);
	}
}
