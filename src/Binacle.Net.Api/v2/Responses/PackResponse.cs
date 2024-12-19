using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Api.v2.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackResponse : v2.Models.ResponseBase<List<v2.Models.BinPackResult>>
{
	internal static PackResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		v2.Requests.PackRequestParameters? parameters,
		Dictionary<string, Lib.Packing.Models.PackingResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		v2.Models.BinPackResultStatus GetResultStatus(Lib.Packing.Models.PackingResult operationResult)
		{
			return operationResult.Status switch
			{
				Lib.Packing.Models.PackingResultStatus.FullyPacked => v2.Models.BinPackResultStatus.FullyPacked,
				Lib.Packing.Models.PackingResultStatus.PartiallyPacked => v2.Models.BinPackResultStatus.PartiallyPacked,
				Lib.Packing.Models.PackingResultStatus.EarlyFail_ContainerDimensionExceeded => v2.Models.BinPackResultStatus.EarlyFail_ContainerDimensionExceeded,
				Lib.Packing.Models.PackingResultStatus.EarlyFail_ContainerVolumeExceeded => v2.Models.BinPackResultStatus.EarlyFail_ContainerVolumeExceeded,
				Lib.Packing.Models.PackingResultStatus.Unknown => v2.Models.BinPackResultStatus.Unknown,
				Lib.Packing.Models.PackingResultStatus.NotPacked => v2.Models.BinPackResultStatus.NotPacked,
				_ => throw new NotImplementedException(),
			};
		}

		var results = new List<v2.Models.BinPackResult>();
		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}


			results.Add(new v2.Models.BinPackResult
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
				PackedItems = operationResult.PackedItems?.Select(x => new v2.Models.ResultBox
				{
					ID = x.ID,
					Dimensions = new Models.Dimensions(x.Dimensions),
					Coordinates = new Models.Coordinates(x.Coordinates!.Value)
				}).ToList(),
				UnpackedItems = operationResult.UnpackedItems?.Select(x => new v2.Models.ResultBox
				{
					ID = x.ID,
					Dimensions = new Models.Dimensions(items.FirstOrDefault(item => item.ID == x.ID)!)
				}).ToList()
			});
		}

		var resultStatus = results.Any(x => 
			x.Result == Models.BinPackResultStatus.FullyPacked
		) ? Models.ResultType.Success : Models.ResultType.Failure;

		return new PackResponse()
		{
			Data = results,
			Result = resultStatus
		};
	}


	internal static PackResponse Create(List<v2.Models.BinPackResult> results)
	{
		return new PackResponse
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	private static Models.ResultType CalculateResultType(List<v2.Models.BinPackResult> results)
	{
		var isSuccess = results.Any(x =>
			x.Result == Models.BinPackResultStatus.FullyPacked
			|| x.Result == Models.BinPackResultStatus.PartiallyPacked
		);
		
		return isSuccess ? Models.ResultType.Success : Models.ResultType.Failure;
	}
}


