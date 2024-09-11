using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.v2.Requests.Examples;

internal class PresetQueryRequestExample : SingleSwaggerExamplesProvider<PresetQueryRequest>
{
	public override PresetQueryRequest GetExample()
	{
		return new PresetQueryRequest
		{
			Parameters = new QueryRequestParameters
			{
				FindSmallestBinOnly = false,
				ReportFittedItems = true,
				ReportUnfittedItems = true
			},
			Items = [
				new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
				new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
				new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
			]
		};
	}
}
