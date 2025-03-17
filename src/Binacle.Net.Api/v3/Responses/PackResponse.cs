using Binacle.Net.Lib.Abstractions.Models;
using Binacle.ViPaq;

namespace Binacle.Net.Api.v3.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackResponse : v3.Models.ResponseBase<List<v3.Models.BinPackResult>>
{
	internal static PackResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		v3.Requests.PackRequestParameters? parameters,
		IDictionary<string, Lib.Packing.Models.PackingResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		v3.Models.BinPackResultStatus GetResultStatus(Lib.Packing.Models.PackingResult operationResult)
		{
			return operationResult.Status switch
			{
				Lib.Packing.Models.PackingResultStatus.FullyPacked => v3.Models.BinPackResultStatus.FullyPacked,
				Lib.Packing.Models.PackingResultStatus.PartiallyPacked => v3.Models.BinPackResultStatus.PartiallyPacked,
				Lib.Packing.Models.PackingResultStatus.EarlyFail_ContainerDimensionExceeded => v3.Models.BinPackResultStatus.EarlyFail_ContainerDimensionExceeded,
				Lib.Packing.Models.PackingResultStatus.EarlyFail_ContainerVolumeExceeded => v3.Models.BinPackResultStatus.EarlyFail_ContainerVolumeExceeded,
				Lib.Packing.Models.PackingResultStatus.Unknown => v3.Models.BinPackResultStatus.Unknown,
				Lib.Packing.Models.PackingResultStatus.NotPacked => v3.Models.BinPackResultStatus.NotPacked,
				_ => throw new NotImplementedException(),
			};
		}

		var results = new List<v3.Models.BinPackResult>();
		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}

			var result = new v3.Models.BinPackResult
			{
				Bin = new v3.Models.Bin
				{
					ID = bin.ID,
					Height = bin.Height,
					Length = bin.Length,
					Width = bin.Width
				},
				Result = GetResultStatus(operationResult),
				PackedBinVolumePercentage = operationResult.PackedBinVolumePercentage,
				PackedItemsVolumePercentage = operationResult.PackedItemsVolumePercentage,
				PackedItems = operationResult.PackedItems?
					.Select(x => new v3.Models.PackedBox()
					{
						ID = x.ID,
						Length = x.Dimensions.Length,
						Width = x.Dimensions.Width,
						Height = x.Dimensions.Height,
						X = x.Coordinates!.Value.X,
						Y = x.Coordinates!.Value.Y,
						Z = x.Coordinates!.Value.Z
					}).ToList(),
				UnpackedItems = operationResult.UnpackedItems?
					.GroupBy(x => x.ID)
					.Select(x => new v3.Models.UnpackedBox
					{
						ID = x.Key,
						Quantity = x.Count()
					}).ToList()
			};

			if (result.PackedItems is not null && result.PackedItems.Any())
			{
				var serializedResult = ViPaqSerializer.SerializeInt32(result.Bin, result.PackedItems!);
				result.ViPaqData = Convert.ToBase64String(serializedResult);
			}

			results.Add(result);
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


	internal static PackResponse Create(List<v3.Models.BinPackResult> results)
	{
		return new PackResponse
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	private static Models.ResultType CalculateResultType(List<v3.Models.BinPackResult> results)
	{
		var isSuccess = results.Any(x =>
			x.Result == Models.BinPackResultStatus.FullyPacked
			|| x.Result == Models.BinPackResultStatus.PartiallyPacked
		);
		
		return isSuccess ? Models.ResultType.Success : Models.ResultType.Failure;
	}
}


