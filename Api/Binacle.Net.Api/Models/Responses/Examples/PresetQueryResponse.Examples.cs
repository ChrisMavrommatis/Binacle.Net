using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.Models.Responses.Examples;

public class PresetQueryResponseExamples : MultipleSwaggerExamplesProvider<QueryResponse>
{
	public override IEnumerable<ISwaggerExample<QueryResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("Success", "Found Bin", "Response example when a bin is found", new QueryResponse
		{
			Result = ResultType.Success,
			Bin = new() { ID = "Small", Length = 10, Width = 40, Height = 60 },
		});

		yield return SwaggerExample.Create("Failure", "Bin not Found", "Response example when a bin is not found", new QueryResponse
		{
			Result = ResultType.Failure,
			Message = "Failed to find bin. Reason: ItemDimensionExceeded"
		});
	}
}
