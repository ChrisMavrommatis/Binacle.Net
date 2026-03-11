using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

public class FitCustomBinRequest : CustomBinRequestBase;

internal class FitCustomBinRequestValidator : AbstractValidator<FitCustomBinRequest>
{
	public FitCustomBinRequestValidator()
	{
		Include(new OperationParametersValidator());
		Include(new BinValidator());
		Include(new ItemsValidator());
	}
}

internal class FitCustomBinRequestExample : ISingleOpenApiExamplesProvider<FitCustomBinRequest>
{
	public IOpenApiExample<FitCustomBinRequest> GetExample()
	{
		return OpenApiExample.Create(
			"FitCustomBinRequest",
			"Fit Custom Bin Request",
			new FitCustomBinRequest()
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
			}
		);
	}
}

internal class FitCustomBinResponseExamples : IMultipleOpenApiExamplesProvider<FitBinResponse>
{
	public IEnumerable<IOpenApiExample<FitBinResponse>> GetExamples()
	{
		yield return OpenApiExample.Create(
			"fitResponse",
			"Fit Response",
			"Example response when all items fit into the bin and no items are left unpacked.",
			new FitBinResponse
			{
				Status = BinFitResultStatus.Fits,
				EarlyExitReason = BinFitEarlyExitReason.None,
				Bin = Bin.From("custom_bin", 10, 40, 60),
				AlgorithmUsed = "FFD",
				PackedItems =
				[
					PackedBox.From("box_2", 10, 12, 15, 0, 0, 0),
					PackedBox.From("box_3", 10, 12, 15, 0, 12, 0),
					PackedBox.From("box_1", 2, 5, 10, 0, 0, 15),
					PackedBox.From("box_1", 2, 5, 10, 0, 24, 0),
				],
				UnpackedItems = [],
				PackedItemsVolumePercentage = 100,
				PackedBinVolumePercentage = 15.83m,
				ViPaqData = "AAQACig8CgwPAAAACgwPAAwAAgUKAAAPAgUKABgA"
			});

		yield return OpenApiExample.Create(
			"doesNotFitResponse",
			"Does Not Fit Response",
			"Example response when some items don't fit into the bin.",
			new FitBinResponse()
			{
				Status = BinFitResultStatus.DoesNotFit,
				EarlyExitReason = BinFitEarlyExitReason.None,
				Bin = Bin.From("custom_bin", 10, 40, 60),
				AlgorithmUsed = "FFD",
				PackedItems =
				[
					PackedBox.From("box_2", 10, 12, 15, 0, 0, 0),
					PackedBox.From("box_3", 10, 12, 15, 0, 12, 0),
				],
				UnpackedItems =
				[
					UnpackedBox.From("box_1", 2)
				],
				PackedItemsVolumePercentage = 79.37m,
				PackedBinVolumePercentage = 12.58m,
				ViPaqData = "AAQACig8CgwPAAAACgwPAAwAAgUKAAAPAgUKABgA"
			});

		yield return OpenApiExample.Create(
			"earlyExitResponse",
			"Early Exit Response",
			"Example response when the early exit condition is met and the algorithm exits before starting.",
			new FitBinResponse()
			{
				Status = BinFitResultStatus.EarlyExit,
				EarlyExitReason = BinFitEarlyExitReason.ContainerDimensionExceeded,
				Bin = Bin.From("custom_bin", 10, 40, 60),
				AlgorithmUsed = "FFD",
				PackedItems = [],
				UnpackedItems =[
					UnpackedBox.From("large_box", 1),
				],
				PackedItemsVolumePercentage = 0,
				PackedBinVolumePercentage = 0,
			});
	}
}
