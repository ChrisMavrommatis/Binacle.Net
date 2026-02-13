using System.Text.Json.Serialization;

namespace Binacle.Net.v4.Contracts;

public class BinPackResponse
{
	
}


public class BinPackResult
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public required OperationResultStatus Status { get; set; }
	public required Bin Bin { get; set; }
	public List<PackedBox>? PackedItems { get; set; }
	public List<UnpackedBox>? UnpackedItems { get; set; }
	public required decimal PackedItemsVolumePercentage { get; set; }
	public required decimal PackedBinVolumePercentage { get; set; }
	public string? ViPaqData { get; set; }
}
