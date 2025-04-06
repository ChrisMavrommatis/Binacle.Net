using Binacle.Net.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v2.Requests.Examples;

internal class PresetPackRequestExample : ISingleOpenApiExamplesProvider<PresetPackRequest>
{
	public IOpenApiExample<PresetPackRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Preset Pack Request",
			new PresetPackRequest
			{
				Parameters = new PackRequestParameters
				{
					NeverReportUnpackedItems = false,
					OptInToEarlyFails = false,
					StopAtSmallestBin = false,
					ReportPackedItemsOnlyWhenFullyPacked = false
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
