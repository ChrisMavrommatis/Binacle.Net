using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinFitResult
{
	public Bin Bin { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinFitResultStatus Result { get; internal set; }

	public List<ResultBox>? FittedItems { get; internal set; }
	public List<ResultBox>? UnfittedItems { get; internal set; }

	public decimal? FittedItemsVolumePercentage { get; internal set; }
	public decimal? FittedBinVolumePercentage { get; internal set; }
}
