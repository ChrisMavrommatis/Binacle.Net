using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib.Packing.Models;

public class PackingParameters : IPackingParameters
{
	public required bool OptInToEarlyFails { get; init; }
	public required bool ReportPackedItemsOnlyWhenFullyPacked { get; init; }
	public required bool NeverReportUnpackedItems { get; init; }
}
