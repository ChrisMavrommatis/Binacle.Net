using System.Text.Json.Serialization;

namespace Binacle.Net.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPackResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinPackResultStatus Result { get; set; }
	public required Bin Bin { get; set; }

	public List<ResultBox>? PackedItems { get; set; }
	public List<ResultBox>? UnpackedItems { get; set; }

	public decimal? PackedItemsVolumePercentage { get; set; }
	public decimal? PackedBinVolumePercentage { get; set; }
}
