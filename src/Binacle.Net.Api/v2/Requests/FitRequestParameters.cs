namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


public interface IWithFitRequestParameters
{
	FitRequestParameters? Parameters { get; set; }
}

public class FitRequestParameters
{
	public bool? ReportFittedItems { get; set; }
	public bool? ReportUnfittedItems { get; set; }
	public bool? FindSmallestBinOnly { get; set; }
}
