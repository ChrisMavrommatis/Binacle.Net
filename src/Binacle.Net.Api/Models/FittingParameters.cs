namespace Binacle.Net.Api.Models;

public class FittingParameters
{
	public bool ReportFittedItems { get; set; }
	public bool ReportUnfittedItems { get; set; }
	public bool FindSmallestBinOnly { get; set; }
}

public class PackingParameters
{
	public bool DontReportItemsOnFail { get; set; }
	public bool IgnoreEarlyFails { get; set; }
}
