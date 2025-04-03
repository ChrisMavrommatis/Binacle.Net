using Binacle.Net.Api.v3.Models;
using Binacle.Net.Api.v3.Requests;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;
using Binacle.ViPaq;

namespace Binacle.Net.Api.v3.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackResponse : ResponseBase<List<BinPackResult>>
{
	internal static PackResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		PackRequestParameters? parameters,
		IDictionary<string, PackingResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
		BinPackResultStatus GetResultStatus(PackingResult operationResult)
		{
			return operationResult.Status switch
			{
				PackingResultStatus.FullyPacked => BinPackResultStatus.FullyPacked,
				PackingResultStatus.PartiallyPacked => BinPackResultStatus.PartiallyPacked,
				PackingResultStatus.EarlyFail_ContainerDimensionExceeded => BinPackResultStatus.EarlyFail_ContainerDimensionExceeded,
				PackingResultStatus.EarlyFail_ContainerVolumeExceeded => BinPackResultStatus.EarlyFail_ContainerVolumeExceeded,
				PackingResultStatus.Unknown => BinPackResultStatus.Unknown,
				PackingResultStatus.NotPacked => BinPackResultStatus.NotPacked,
				_ => throw new NotImplementedException(),
			};
		}

		var results = new List<BinPackResult>();
		for (var i = 0; i < bins.Count; i++)
		{
			var bin = bins[i];
			if (!operationResults.TryGetValue(bin.ID, out var operationResult))
			{
				continue;
			}

			var result = new BinPackResult
			{
				Bin = new Bin
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
					.Select(x => new PackedBox()
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
					.Select(x => new UnpackedBox
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
			x.Result == BinPackResultStatus.FullyPacked
		) ? ResultType.Success : ResultType.Failure;

		return new PackResponse()
		{
			Data = results,
			Result = resultStatus
		};
	}


	internal static PackResponse Create(List<BinPackResult> results)
	{
		return new PackResponse
		{
			Data = results,
			Result = CalculateResultType(results)
		};
	}

	private static ResultType CalculateResultType(List<BinPackResult> results)
	{
		var isSuccess = results.Any(x =>
			x.Result == BinPackResultStatus.FullyPacked
			|| x.Result == BinPackResultStatus.PartiallyPacked
		);
		
		return isSuccess ? ResultType.Success : ResultType.Failure;
	}
}


