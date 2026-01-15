using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class FitResponse : ResponseBase<List<BinFitResult>>
{
	internal static FitResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		FitRequestParameters parameters,
		IDictionary<string, OperationResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		BinFitResultStatus GetResultStatus(OperationResult operationResult)
		{
			return operationResult.Status switch
			{
				OperationResultStatus.FullyPacked => BinFitResultStatus.AllItemsFit,
				OperationResultStatus.PartiallyPacked => BinFitResultStatus.NotAllItemsFit,
				OperationResultStatus.EarlyFail_ContainerDimensionExceeded => BinFitResultStatus.EarlyFail_ItemDimensionExceeded,
				OperationResultStatus.EarlyFail_ContainerVolumeExceeded => BinFitResultStatus.EarlyFail_TotalVolumeExceeded,
				OperationResultStatus.NotPacked => BinFitResultStatus.NotAllItemsFit,
				_ => throw new NotSupportedException($"No Implementation exists for operation result  status {operationResult.Status.ToString()}"),
			};
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
				FittedBinVolumePercentage  = operationResult.PackedBinVolumePercentage,
				FittedItemsVolumePercentage  = operationResult.PackedItemsVolumePercentage,
				FittedItems  = operationResult.PackedItems?
					.Select(x => new FittedBox()
					{
						ID = x.ID,
						Length = x.Dimensions.Length,
						Width = x.Dimensions.Width,
						Height = x.Dimensions.Height,
					}).ToList(),
				UnfittedItems = operationResult.UnpackedItems?				
					.Select(x => new UnfittedBox{
						ID = x.ID,
						Quantity = x.Quantity
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
