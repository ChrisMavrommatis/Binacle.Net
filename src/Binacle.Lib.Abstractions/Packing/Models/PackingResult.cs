namespace Binacle.Lib.Packing.Models;

public class PackingResult
{
	internal PackingResult()
	{
		
	}

	public required string BinID { get; init; }
	public PackingResultStatus Status { get; internal set; }

	public List<ResultItem>? PackedItems { get; internal set; }
	public List<ResultItem>? UnpackedItems { get; internal set; }

	public decimal PackedItemsVolumePercentage { get; internal set; }
	public decimal PackedBinVolumePercentage { get; internal set; }
}
