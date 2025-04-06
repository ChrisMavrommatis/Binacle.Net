using Binacle.Net.v1.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v1.Requests.Examples;

internal class PresetQueryRequestExample : ISingleOpenApiExamplesProvider<PresetQueryRequest>
{
	public IOpenApiExample<PresetQueryRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Preset Query Request",
			new PresetQueryRequest
			{
				Items = new List<Box>
				{
					new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
					new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
					new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
				}
			}
		);
	}
}
