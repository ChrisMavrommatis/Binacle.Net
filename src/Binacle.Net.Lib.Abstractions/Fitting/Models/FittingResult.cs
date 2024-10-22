namespace Binacle.Net.Lib.Fitting.Models;
public class FittingResult
{
	internal FittingResult()
	{

	}

	public required string BinID { get; init; }
	public FittingResultStatus Status { get; internal set; }
	public FittingFailedResultReason? Reason { get; internal set; }

	public List<ResultItem>? FittedItems { get; internal set; }
	public List<ResultItem>? UnfittedItems { get; internal set; }

	public decimal? FittedItemsVolumePercentage { get; internal set; }
	public decimal? FittedBinVolumePercentage { get; internal set; }
}
