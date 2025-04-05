using Binacle.Net.Api.v2.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.v2.Requests.Examples;

internal class CustomFitRequestExample : ISingleOpenApiExamplesProvider<CustomFitRequest>
{
	public IOpenApiExample<CustomFitRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Custom Fit Request",
			new CustomFitRequest
			{
				Parameters = new FitRequestParameters
				{
					FindSmallestBinOnly = false,
					ReportFittedItems = true,
					ReportUnfittedItems = true
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
