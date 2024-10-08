namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class PackRequestParameters
{
	public bool? OptInToEarlyFails { get; set; }
	public bool? ReportPackedItemsOnlyWhenFullyPacked { get; set; }
	public bool? NeverReportUnpackedItems { get; set; }
	public bool? StopAtSmallestBin { get; set; }
}
