namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class QueryRequestParameters
{
	public bool? ReportFittedItems { get; set; }
	public bool? ReportUnfittedItems { get; set; }
	public bool? FindSmallestBinOnly { get; set; }
}
