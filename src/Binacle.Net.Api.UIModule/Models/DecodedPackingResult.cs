namespace Binacle.Net.Api.UIModule.Models;

internal class DecodedPackingResult
{
	public required string EncodedResult { get; init; }
	public Bin? Bin { get; init; }
	public List<PackedItem>? PackedItems { get; init; }
	
	public double? PackedBinVolumePercentage { get; init; }
}
