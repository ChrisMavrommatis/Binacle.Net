using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.v2.Responses.Examples;

internal class PresetQueryResponseExamples : MultipleSwaggerExamplesProvider<QueryResponse>
{
	public override IEnumerable<ISwaggerExample<QueryResponse>> GetExamples()
	{
		// TODO Create Example response

		yield return SwaggerExample.Create("Success", "Found Bin", "Response example when a bin is found", new QueryResponse
		{
			//Result = ResultType.Success,
			//Data = new() { ID = "Small", Length = 10, Width = 40, Height = 60 },
		});

		yield return SwaggerExample.Create("Failure", "Bin not Found", "Response example when a bin is not found", new QueryResponse
		{
			//Result = ResultType.Failure,
			//Message = "Failed to find bin. Reason: ItemDimensionExceeded"
		});
	}
}
