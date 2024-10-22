namespace Binacle.Net.Api.UIModule.Models;

internal class PackingResult
{
	public PackResultType Result { get; set; }
	public Bin? Bin { get; set; }
	public List<PackedItem>? PackedItems { get; set; }
	public List<PackedItem>? UnpackedItems { get; set; }
	public decimal? PackedItemsVolumePercentage { get; set; }
	public decimal? PackedBinVolumePercentage { get; set; }
}
