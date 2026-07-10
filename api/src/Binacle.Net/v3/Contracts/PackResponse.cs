using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.ViPaq;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackResponse : ResponseBase<List<BinPackResult>>
{
	internal static BinPackResultStatus MapResultStatus(OperationResultStatus resultStatus, EarlyExitReason earlyExitReason)
	{
		if (resultStatus == OperationResultStatus.EarlyExit)
		{
			return earlyExitReason switch
			{
				EarlyExitReason.ContainerDimensionExceeded => BinPackResultStatus.EarlyFail_ContainerDimensionExceeded,
				EarlyExitReason.ContainerVolumeExceeded => BinPackResultStatus.EarlyFail_ContainerVolumeExceeded,
				_ => throw new NotSupportedException($"No Implementation exists for operation result early exit reason {earlyExitReason.ToString()}"),
			};
		}
		return resultStatus switch
		{
			OperationResultStatus.FullyPacked => BinPackResultStatus.FullyPacked,
			OperationResultStatus.PartiallyPacked => BinPackResultStatus.PartiallyPacked,
			OperationResultStatus.NotPacked => BinPackResultStatus.NotPacked,
			_ => throw new NotSupportedException($"No Implementation exists for operation result  status {resultStatus.ToString()}"),
		};
	}

	
	internal static PackResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		PackRequestParameters parameters,
		IDictionary<string, OperationResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
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
				Result = MapResultStatus(operationResult.Status, operationResult.EarlyExitReason),
				PackedBinVolumePercentage = operationResult.PackedBinVolumePercentage,
				PackedItemsVolumePercentage = operationResult.PackedItemsVolumePercentage,
				PackedItems = operationResult.PackedItems?
					.Select(x => new PackedBox()
					{
						ID = x.ID,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height,
						X = x.X,
						Y = x.Y,
						Z = x.Z
					}).ToList(),
				UnpackedItems = operationResult.UnpackedItems?
					.Select(x => new UnpackedBox
					{
						ID = x.ID,
						Quantity = x.Quantity
					}).ToList()
			};

			if (parameters.IncludeViPaqData)
			{
				if (result.PackedItems is not null && result.PackedItems.Count > 0)
				{
					result.ViPaqData = ViPaqSerializer
						.Serialize<Bin, PackedBox, int>(result.Bin, result.PackedItems!)
						.ToBase64();
				}				
			}
			
			results.Add(result);
		}

		return Create(results);
	}


	internal static PackResponse Create(List<BinPackResult> results)
	{
		var isSuccess = results.Any(x =>
			x.Result == BinPackResultStatus.FullyPacked
		);
		
		return new PackResponse
		{
			Data = results,
			Result = isSuccess ? ResultType.Success : ResultType.Failure
		};
	}
}



public class BinPackResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required BinPackResultStatus Result { get; set; }
	public required Bin Bin { get; set; }

	public List<PackedBox>? PackedItems { get; set; }
	public List<UnpackedBox>? UnpackedItems { get; set; }

	public required decimal PackedItemsVolumePercentage { get; set; }
	public required decimal PackedBinVolumePercentage { get; set; }
	
	public string? ViPaqData { get; set; }
}

public enum BinPackResultStatus
{
	Unknown,
	NotPacked,
	PartiallyPacked,
	FullyPacked,
	EarlyFail_ContainerVolumeExceeded,
	EarlyFail_ContainerDimensionExceeded,
}


public class PackedBox :
	IWithID,
	IWithDimensions,
	IWithCoordinates
{
	public required string ID { get; set; }
	public required int Length { get; set; }
	public required int Width { get; set; }
	public required int Height { get; set; }
	public required int X { get; set; }
	public required int Y { get; set; }
	public required int Z { get; set; }
}

public class UnpackedBox : IWithID
{
	public required string ID { get; set; }
	public required int Quantity { get; set; }
}
