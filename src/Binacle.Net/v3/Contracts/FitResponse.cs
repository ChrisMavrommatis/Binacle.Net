using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class FitResponse : ResponseBase<List<BinFitResult>>
{
	internal static FitResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		FitRequestParameters parameters,
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

			var result = new BinFitResult
			{
				Bin = new Bin
				{
					ID = bin.ID,
					Height = bin.Height,
					Length = bin.Length,
					Width = bin.Width
				},
				Result = GetResultStatus(operationResult),
				FittedBinVolumePercentage  = operationResult.FittedBinVolumePercentage,
				FittedItemsVolumePercentage  = operationResult.FittedItemsVolumePercentage,
				FittedItems  = operationResult.FittedItems?
					.Select(x => new FittedBox()
					{
						ID = x.ID,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height,
					}).ToList(),
				UnfittedItems = operationResult.UnfittedItems?				
					.GroupBy(x => x.ID)
					.Select(x => new UnfittedBox{
						ID = x.Key,
						Quantity = x.Count()
					}).ToList()
			};

			results.Add(result);
		}

		return Create(results);
	}

	internal static FitResponse Create(List<BinFitResult> results)
	{
		var isSuccess = results.Any(x =>
			x.Result == BinFitResultStatus.AllItemsFit
		);

		return new FitResponse
		{
			Data = results,
			Result = isSuccess ? ResultType.Success : ResultType.Failure
		};
	}
}



public class BinFitResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required BinFitResultStatus  Result { get; set; }
	public required Bin Bin { get; set; }

	public List<FittedBox>? FittedItems { get; set; }
	public List<UnfittedBox>? UnfittedItems { get; set; }

	public decimal? FittedBinVolumePercentage  { get; set; }
	public decimal? FittedItemsVolumePercentage  { get; set; }
}

public enum BinFitResultStatus
{
	AllItemsFit,
	NotAllItemsFit,
	EarlyFail_TotalVolumeExceeded,
	EarlyFail_ItemDimensionExceeded
}

public class FittedBox : 
	IWithID, 
	IWithDimensions
{
	public required string ID { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required int Height { get; set; }
}

public class UnfittedBox : IWithID
{
	public required string ID { get; set; }
	public required int Quantity { get; set; }
}
