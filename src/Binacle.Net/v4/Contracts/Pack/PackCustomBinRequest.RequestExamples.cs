using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591
internal class PackCustomBinRequestExample : ISingleOpenApiExamplesProvider<PackCustomBinRequest>
{
	public IOpenApiExample<PackCustomBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packCustomBinRequest",
			"Pack Custom Bin Request",
			new PackCustomBinRequest()
			{
				Parameters = new OperationParameters()
				{
					Algorithm = Algorithm.Best,
					IncludeViPaqData = true,
				},
				Bin = Bin.From("custom_bin", 10, 40, 60),
				Items =
				[
					Box.From("box_1", 2, 5, 10, 2),
					Box.From("box_2", 12, 15, 10, 1),
					Box.From("box_3", 12, 10, 15, 1),
				]
			});
	}
}
