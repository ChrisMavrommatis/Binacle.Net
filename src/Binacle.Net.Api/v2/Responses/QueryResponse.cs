using Binacle.Net.Lib.Abstractions.Models;
using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


namespace Binacle.Net.Api.v2.Responses;

public class QueryResponse : ResponseBase<List<BinFitResult>>
{
	internal static QueryResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		v2.Requests.QueryRequestParameters? parameters,
		Dictionary<string, Lib.Fitting.Models.FittingResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		FitResult GetResultStatus(Lib.Fitting.Models.FittingResult operationResult)
		{
			if (operationResult.Status == Lib.Fitting.Models.FittingResultStatus.Success)
			{
				return FitResult.AllItemsFit;
			}

			if (operationResult.Reason == Lib.Fitting.Models.FittingFailedResultReason.TotalVolumeExceeded)
			{
				return FitResult.EarlyFail_TotalVolumeExceeded;
			}
			if (operationResult.Reason == Lib.Fitting.Models.FittingFailedResultReason.ItemDimensionExceeded)
			{
				return FitResult.EarlyFail_ItemDimensionExceeded;
			}

			return FitResult.NotAllItemsFit;
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
				Bin = new v2.Models.Bin
				{
					ID = bin.ID,
					Height = bin.Height,
					Length = bin.Length,
					Width = bin.Width
				},
				Result = GetResultStatus(operationResult),
				PackedBinVolumePercentage = operationResult.PackedBinVolumePercentage,
				PackedItemsVolumePercentage = operationResult.PackedItemsVolumePercentage,
				FittedItems = operationResult.FittedItems?.Select(x => new v2.Models.Box
				{
					ID = x.ID,
					Height = x.Height,
					Length = x.Length,
					Width = x.Width
				}).ToList(),
				UnfittedItems = operationResult.UnfittedItems?.Select(x => new v2.Models.Box
				{
					ID = x.ID,
					Height = x.Height,
					Length = x.Length,
					Width = x.Width
				}).ToList()
			});
		}

		// TODO: maybe refactor this result type
		return new QueryResponse() 
		{ 
			Data = results, 
			Result = Models.ResultType.Success
		};
	}
}
public class BinFitResult
{
	public v2.Models.Bin Bin { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public FitResult Result { get; internal set; }

	public List<v2.Models.Box>? FittedItems { get; internal set; }
	public List<v2.Models.Box>? UnfittedItems { get; internal set; }

	public decimal? PackedItemsVolumePercentage { get; internal set; }
	public decimal? PackedBinVolumePercentage { get; internal set; }
}

public enum FitResult
{
	AllItemsFit,
	NotAllItemsFit,
	EarlyFail_TotalVolumeExceeded,
	EarlyFail_ItemDimensionExceeded
}

