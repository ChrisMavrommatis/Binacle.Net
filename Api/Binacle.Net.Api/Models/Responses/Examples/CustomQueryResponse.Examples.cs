using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.Models.Responses.Examples;

public class CustomQueryResponseExamples : MultipleSwaggerExamplesProvider<QueryResponse>
{
	public override IEnumerable<ISwaggerExample<QueryResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("success", "Found Bin", "Response example when a bin is found", new QueryResponse
		{
			Result = ResultType.Success,
			Bin = new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
		});

		yield return SwaggerExample.Create("failure", "Bin not Found", "Response example when a bin is not found", new QueryResponse
		{
			Result = ResultType.Failure,
			Message = "Failed to find bin. Reason: ItemDimensionExceeded"
		});
	}
}

