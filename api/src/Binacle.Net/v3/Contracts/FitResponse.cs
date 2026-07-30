using System.Text.Json.Serialization;
using Binacle.Lib.Abstractions.Models;
using System.ComponentModel;
using Binacle.Net.Kernel.OpenApi.Attributes;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Description("The fit results.")]
public class FitResponse : ResponseBase<List<BinFitResult>>
{
	internal static BinFitResultStatus MapResultStatus(OperationResultStatus resultStatus, EarlyExitReason earlyExitReason)
	{
		if (resultStatus == OperationResultStatus.EarlyExit)
		{
			return earlyExitReason switch
			{
				EarlyExitReason.ContainerDimensionExceeded => BinFitResultStatus.EarlyFail_ItemDimensionExceeded,
				EarlyExitReason.ContainerVolumeExceeded => BinFitResultStatus.EarlyFail_TotalVolumeExceeded,
				_ => throw new NotSupportedException($"No Implementation exists for operation result early exit reason {earlyExitReason.ToString()}"),
			};
		}
		return resultStatus switch
		{
			OperationResultStatus.FullyPacked => BinFitResultStatus.AllItemsFit,
			OperationResultStatus.PartiallyPacked => BinFitResultStatus.NotAllItemsFit,
			OperationResultStatus.NotPacked => BinFitResultStatus.NotAllItemsFit,
			_ => throw new NotSupportedException($"No Implementation exists for operation result  status {resultStatus.ToString()}"),
		};
	}
	
	internal static FitResponse Create<TBin, TItem>(
		List<TBin> bins,
		List<TItem> items,
		FitRequestParameters parameters,
		IDictionary<string, OperationResult> operationResults
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions
	{
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
				Result = MapResultStatus(operationResult.Status, operationResult.EarlyExitReason),
				FittedBinVolumePercentage  = operationResult.PackedBinVolumePercentage,
				FittedItemsVolumePercentage  = operationResult.PackedItemsVolumePercentage,
				FittedItems  = operationResult.PackedItems?
					.Select(x => new FittedBox()
					{
						ID = x.ID,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height,
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



[Description("The fit result for one bin.")]
public class BinFitResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Description(SchemaDescriptions.Result)]
	public required BinFitResultStatus  Result { get; set; }
	[Description(SchemaDescriptions.Bin)]
	public required Bin Bin { get; set; }

	[Description(SchemaDescriptions.FittedItems)]
	public List<FittedBox>? FittedItems { get; set; }
	[Description(SchemaDescriptions.UnfittedItems)]
	public List<UnfittedBox>? UnfittedItems { get; set; }

	[Description(SchemaDescriptions.FittedBinVolumePercentage)]
	[OpenApiSchemaRange(Minimum = 0, Maximum = 100)]
	public decimal? FittedBinVolumePercentage  { get; set; }
	[Description(SchemaDescriptions.FittedItemsVolumePercentage)]
	[OpenApiSchemaRange(Minimum = 0, Maximum = 100)]
	public decimal? FittedItemsVolumePercentage  { get; set; }
}

[Description("Outcome of a fit check.")]
public enum BinFitResultStatus
{
	AllItemsFit,
	NotAllItemsFit,
	EarlyFail_TotalVolumeExceeded,
	EarlyFail_ItemDimensionExceeded
}

[Description("An item that fits in the bin.")]
public class FittedBox : 
	IWithID, 
	IWithDimensions
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
}

[Description("An item that does not fit in the bin.")]
public class UnfittedBox : IWithID
{
	[Description(SchemaDescriptions.Id)]
	public required string ID { get; set; }
	[Description(SchemaDescriptions.Quantity)]
	[OpenApiSchemaRange(Minimum = 1)]
	public required int Quantity { get; set; }
}
