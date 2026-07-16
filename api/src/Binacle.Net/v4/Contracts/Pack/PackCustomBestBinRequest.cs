using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591

public class PackCustomBestBinRequest : CustomBinsRequestBase;

internal class PackCustomBestBinRequestValidator : AbstractValidator<PackCustomBestBinRequest>
{
	public PackCustomBestBinRequestValidator()
	{
		Include(new CustomBinsRequestBaseValidator());
	}
}

internal class PackCustomBestBinRequestExample : ISingleOpenApiExamplesProvider<PackCustomBestBinRequest>
{
	public IOpenApiExample<PackCustomBestBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"packCustomBestBinRequest",
			"Pack Custom Best Bin Request",
			new PackCustomBestBinRequest()
			{
				Parameters = new OperationParameters()
				{
					Algorithm = Algorithm.Best,
					IncludeViPaqData = true,
				},
				Bins = ExampleData.Bins("custom_bin"),
				Items = ExampleData.Items()
			});
	}
}

internal class PackCustomBestBinResponseExamples : IMultipleOpenApiExamplesProvider<PackBinResponse>
{
	public IEnumerable<IOpenApiExample<PackBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fullyPackedResponse",
			"Fully Packed Response",
			"Example response for the bin whose volume the items fill the most.",
			PackExampleResponses.FullyPacked("custom_bin_1"));

		yield return OpenApiExample.Create(
			"partiallyPackedResponse",
			"Partially Packed Response",
			"Example response when no bin fits every item, so the one the items fill the most is returned.",
			PackExampleResponses.PartiallyPacked("custom_bin_1"));

		yield return OpenApiExample.Create(
			"unpackedResponse",
			"Unpacked Response",
			"Example response when none of the bins can fit the items.",
			PackExampleResponses.NotPacked("custom_bin_1"));
	}
}
