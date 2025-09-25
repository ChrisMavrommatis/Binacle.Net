namespace Binacle.Lib.Abstractions.Algorithms;

public interface IPackingParameters
{
	public bool OptInToEarlyFails { get; }
	public bool ReportPackedItemsOnlyWhenFullyPacked { get; }
	public bool NeverReportUnpackedItems { get; }
}
