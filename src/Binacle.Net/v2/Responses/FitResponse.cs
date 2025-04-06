
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace Binacle.Net.v2.Responses;

public class FitResponse : ResponseBase<List<BinFitResult>>
{
	internal static FitResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		FitRequestParameters? parameters,
		IDictionary<string, FittingResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		BinFitResultStatus GetResultStatus(FittingResult operationResult)
		{
			if (operationResult.Status == FittingResultStatus.Success)
			{
				return BinFitResultStatus.AllItemsFit;
			}

			if (operationResult.Reason == FittingFailedResultReason.TotalVolumeExceeded)
			{
				return BinFitResultStatus.EarlyFail_TotalVolumeExceeded;
			}
			if (operationResult.Reason == FittingFailedResultReason.ItemDimensionExceeded)
			{
				return BinFitResultStatus.EarlyFail_ItemDimensionExceeded;
			}

			return BinFitResultStatus.NotAllItemsFit;
		}


		var results = new List<BinFitResult>();
		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}

			results.Add(new BinFitResult
			{
				Bin = new Bin
				{
					ID = bin.ID,
					Height = bin.Height,
					Length = bin.Length,
					Width = bin.Width
				},
				Result = GetResultStatus(operationResult),
				FittedBinVolumePercentage = operationResult.FittedBinVolumePercentage,
				FittedItemsVolumePercentage = operationResult.FittedItemsVolumePercentage,
				FittedItems = operationResult.FittedItems?.Select(x => new ResultBox
				{
					ID = x.ID,
					Dimensions = new Dimensions(x)
				}).ToList(),
				UnfittedItems = operationResult.UnfittedItems?.Select(x => new ResultBox
				{
					ID = x.ID,
					Dimensions = new Dimensions(x)
				}).ToList()
			});
		}

		return new FitResponse()
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	internal static FitResponse Create(List<BinFitResult> results)
	{
		return new FitResponse
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	private static ResultType CalculateResultType(List<BinFitResult> results)
	{
		return results.Any(x => x.Result == BinFitResultStatus.AllItemsFit) ? ResultType.Success : ResultType.Failure;
	}
}
