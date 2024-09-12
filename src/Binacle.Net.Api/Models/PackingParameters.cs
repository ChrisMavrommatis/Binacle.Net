namespace Binacle.Net.Api.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PackingParameters
{
	public bool OptInToEarlyFails { get; set; }
	public bool ReportPackedItemsOnlyWhenFullyPacked { get; set; }
	public bool NeverReportUnpackedItems { get; set; }
	public bool StopAtSmallestBin { get; set; }
}
