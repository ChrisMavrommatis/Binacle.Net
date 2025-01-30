namespace Binacle.Net.Api.UIModule.Models;

internal class DecodedPackingResult
{
	public required string EncodedResult { get; set; }
	public Bin? Bin { get; set; }
	public List<PackedItem>? PackedItems { get; set; }
}
