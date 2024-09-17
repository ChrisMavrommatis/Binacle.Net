namespace Binacle.Net.Api.UIModule.Models;

internal class PackingResult
{
	public PackResultType Result { get; set; }
	public Bin Bin { get; set; }
	public List<PackedItem>? PackedItems { get; set; }
	public List<PackedItem>? UnpackedItems { get; set; }
	public double? PackedItemsVolumePercentage { get; set; }
	public double? PackedBinVolumePercentage { get; set; }
}
