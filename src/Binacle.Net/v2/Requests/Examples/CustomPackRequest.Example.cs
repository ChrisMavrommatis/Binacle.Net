using Binacle.Net.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v2.Requests.Examples;

internal class CustomPackRequestExample : ISingleOpenApiExamplesProvider<CustomPackRequest>
{
	public IOpenApiExample<CustomPackRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Custom Pack Request",
			new CustomPackRequest
			{
				Parameters = new PackRequestParameters
				{
					NeverReportUnpackedItems = false,
					OptInToEarlyFails = false,
					StopAtSmallestBin = false,
					ReportPackedItemsOnlyWhenFullyPacked = false
				},
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
