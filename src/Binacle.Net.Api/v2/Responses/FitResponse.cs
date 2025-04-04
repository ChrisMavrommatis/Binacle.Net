﻿
using Binacle.Net.Lib.Abstractions.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace Binacle.Net.Api.v2.Responses;

public class FitResponse : v2.Models.ResponseBase<List<v2.Models.BinFitResult>>
{
	internal static FitResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		v2.Requests.FitRequestParameters? parameters,
		IDictionary<string, Lib.Fitting.Models.FittingResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		v2.Models.BinFitResultStatus GetResultStatus(Lib.Fitting.Models.FittingResult operationResult)
		{
			if (operationResult.Status == Lib.Fitting.Models.FittingResultStatus.Success)
			{
				return v2.Models.BinFitResultStatus.AllItemsFit;
			}

			if (operationResult.Reason == Lib.Fitting.Models.FittingFailedResultReason.TotalVolumeExceeded)
			{
				return v2.Models.BinFitResultStatus.EarlyFail_TotalVolumeExceeded;
			}
			if (operationResult.Reason == Lib.Fitting.Models.FittingFailedResultReason.ItemDimensionExceeded)
			{
				return v2.Models.BinFitResultStatus.EarlyFail_ItemDimensionExceeded;
			}

			return v2.Models.BinFitResultStatus.NotAllItemsFit;
		}


		var results = new List<v2.Models.BinFitResult>();
		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}

			results.Add(new v2.Models.BinFitResult
			{
				Bin = new v2.Models.Bin
				{
					ID = bin.ID,
					Height = bin.Height,
					Length = bin.Length,
					Width = bin.Width
				},
				Result = GetResultStatus(operationResult),
				FittedBinVolumePercentage = operationResult.FittedBinVolumePercentage,
				FittedItemsVolumePercentage = operationResult.FittedItemsVolumePercentage,
				FittedItems = operationResult.FittedItems?.Select(x => new v2.Models.ResultBox
				{
					ID = x.ID,
					Dimensions = new Models.Dimensions(x)
				}).ToList(),
				UnfittedItems = operationResult.UnfittedItems?.Select(x => new v2.Models.ResultBox
				{
					ID = x.ID,
					Dimensions = new Models.Dimensions(x)
				}).ToList()
			});
		}

		return new FitResponse()
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	internal static FitResponse Create(List<v2.Models.BinFitResult> results)
	{
		return new FitResponse
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	private static Models.ResultType CalculateResultType(List<v2.Models.BinFitResult> results)
	{
		return results.Any(x => x.Result == Models.BinFitResultStatus.AllItemsFit) ? Models.ResultType.Success : Models.ResultType.Failure;
	}
}
