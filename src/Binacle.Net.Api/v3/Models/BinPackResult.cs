using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v3.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class BinPackResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public BinPackResultStatus Result { get; set; }
	public required Bin Bin { get; set; }

	public List<PackedBox>? PackedItems { get; set; }
	public List<UnpackedBox>? UnpackedItems { get; set; }

	public decimal? PackedItemsVolumePercentage { get; set; }
	public decimal? PackedBinVolumePercentage { get; set; }
	
	public string? SerializedResult { get; set; }
}
