using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinFitResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinFitResultStatus Result { get; set; }

	public Bin Bin { get; set; }

	public List<ResultBox>? FittedItems { get; set; }
	public List<ResultBox>? UnfittedItems { get; set; }

	public decimal? FittedItemsVolumePercentage { get; set; }
	public decimal? FittedBinVolumePercentage { get; set; }
}
