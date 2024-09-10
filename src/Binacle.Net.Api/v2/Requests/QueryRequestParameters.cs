namespace Binacle.Net.Api.v2.Requests;

public class QueryRequestParameters
{
	public bool? ReportFittedItems { get; set; }
	public bool? ReportUnfittedItems { get; set; }
	public bool? FindSmallestBinOnly { get; set; }
}
