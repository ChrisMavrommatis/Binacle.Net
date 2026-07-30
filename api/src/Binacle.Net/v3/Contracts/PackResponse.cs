using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using Binacle.ViPaq;
using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("The packing results.")]
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



[Description("The packing result for one bin.")]
public class BinPackResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Description(SchemaDescriptions.Result)]
	public required BinPackResultStatus Result { get; set; }
	[Description(SchemaDescriptions.Bin)]
	public required Bin Bin { get; set; }

	[Description(SchemaDescriptions.PackedItems)]
	public List<PackedBox>? PackedItems { get; set; }
	[Description(SchemaDescriptions.UnpackedItems)]
	public List<UnpackedBox>? UnpackedItems { get; set; }

	[Description(SchemaDescriptions.PackedItemsVolumePercentage)]
	[OpenApiSchemaRange(Minimum = 0, Maximum = 100)]
	public required decimal PackedItemsVolumePercentage { get; set; }
	[Description(SchemaDescriptions.PackedBinVolumePercentage)]
	[OpenApiSchemaRange(Minimum = 0, Maximum = 100)]
	public required decimal PackedBinVolumePercentage { get; set; }
	
	[Description(SchemaDescriptions.ViPaqData)]
	public string? ViPaqData { get; set; }
}

[Description("Outcome of a packing operation.")]
public enum BinPackResultStatus
{
	Unknown,
	NotPacked,
	PartiallyPacked,
	FullyPacked,
	EarlyFail_ContainerVolumeExceeded,
	EarlyFail_ContainerDimensionExceeded,
}


[Description("An item placed inside the bin, with its position.")]
public class PackedBox :
	IWithID,
	IWithDimensions,
	IWithCoordinates
{
	[Description(SchemaDescriptions.Id)]
	public required string ID { get; set; }
	[Description(SchemaDescriptions.Length)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Length { get; set; }
	[Description(SchemaDescriptions.Width)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Width { get; set; }
	[Description(SchemaDescriptions.Height)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Height { get; set; }
	[Description(SchemaDescriptions.CoordinateX)]
	[OpenApiSchemaRange(Minimum = 0)]
	public required int X { get; set; }
	[Description(SchemaDescriptions.CoordinateY)]
	[OpenApiSchemaRange(Minimum = 0)]
	public required int Y { get; set; }
	[Description(SchemaDescriptions.CoordinateZ)]
	[OpenApiSchemaRange(Minimum = 0)]
	public required int Z { get; set; }
}

[Description("An item that could not be placed in the bin.")]
public class UnpackedBox : IWithID
{
	[Description(SchemaDescriptions.Id)]
	public required string ID { get; set; }
	[Description(SchemaDescriptions.Quantity)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Quantity { get; set; }
}
