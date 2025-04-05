using Binacle.Net.Api.v1.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v1.Requests.Examples;

internal class CustomQueryRequestExample : ISingleOpenApiExamplesProvider<CustomQueryRequest>
{
	public IOpenApiExample<CustomQueryRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Custom Query Request",
			new CustomQueryRequest
			{
				Bins = new List<Bin>
				{
					new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
					new() { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
				},
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
