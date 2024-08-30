namespace Binacle.Net.Lib;

public class BinPackingResult
{
	internal BinPackingResult()
	{
		
	}

	public string BinID { get; internal set; }
	public BinPackingResultStatus Status { get; internal set; }

	public List<ResultItem>? PackedItems { get; internal set; }
	public List<ResultItem>? UnpackedItems { get; internal set; }

	public decimal PackedItemsVolumePercentage { get; internal set; }
	public decimal PackedBinVolumePercentage { get; internal set; }
}
