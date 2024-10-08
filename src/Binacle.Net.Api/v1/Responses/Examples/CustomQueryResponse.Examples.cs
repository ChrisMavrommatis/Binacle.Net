﻿using ChrisMavrommatis.SwaggerExamples;
using ChrisMavrommatis.SwaggerExamples.Abstractions;

namespace Binacle.Net.Api.v1.Responses.Examples;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomQueryResponseExamples : MultipleSwaggerExamplesProvider<QueryResponse>
{
	public override IEnumerable<ISwaggerExample<QueryResponse>> GetExamples()
	{
		yield return SwaggerExample.Create("success", "Found Bin", "Response example when a bin is found", new QueryResponse
		{
			Result = v1.Models.ResultType.Success,
			Bin = new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
		});

		yield return SwaggerExample.Create("failure", "Bin not Found", "Response example when a bin is not found", new QueryResponse
		{
			Result = v1.Models.ResultType.Failure,
			Message = "Failed to find bin. Reason: ItemDimensionExceeded"
		});
	}
}

