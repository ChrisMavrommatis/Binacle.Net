using Binacle.Net.Api.Models;
using Binacle.Net.Api.v2.Models;
using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.v2.Responses.Examples;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomQueryResponseExamples : MultipleSwaggerExamplesProvider<Response<Bin?>>
{
	public override IEnumerable<ISwaggerExample<Response<Bin?>>> GetExamples()
	{
		yield return SwaggerExample.Create("success", "Found Bin", "Response example when a bin is found", new Response<Bin?>
		{
			Result = ResultType.Success,
			Data = new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
		});

		yield return SwaggerExample.Create("failure", "Bin not Found", "Response example when a bin is not found", new Response<Bin?>
		{
			Result = ResultType.Failure,
			Message = "Failed to find bin. Reason: ItemDimensionExceeded"
		});
	}
}

