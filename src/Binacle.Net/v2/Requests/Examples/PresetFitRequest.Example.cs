using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v2.Requests.Examples;

internal class PresetFitRequestExample : ISingleOpenApiExamplesProvider<PresetFitRequest>
{
	public IOpenApiExample<PresetFitRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Preset Fit Request",
			new PresetFitRequest
			{
				Parameters = new FitRequestParameters
				{
					FindSmallestBinOnly = false,
					ReportFittedItems = true,
					ReportUnfittedItems = true
				},
				Items =
				[
					new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
					new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
					new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
				]
			}
		);
	}
}
