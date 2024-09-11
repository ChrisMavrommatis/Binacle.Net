using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPackResult
{
	public Bin Bin { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinPackResultStatus Result { get; internal set; }

	public List<ResultBox>? PackedItems { get; internal set; }
	public List<ResultBox>? UnpackedItems { get; internal set; }

	public decimal? PackedItemsVolumePercentage { get; internal set; }
	public decimal? PackedBinVolumePercentage { get; internal set; }
}
