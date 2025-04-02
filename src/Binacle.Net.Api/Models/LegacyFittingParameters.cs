using Binacle.Net.Api.Kernel.Logs.Models;

namespace Binacle.Net.Api.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class LegacyFittingParameters : ILogConvertible
{
	public bool ReportFittedItems { get; init; }
	public bool ReportUnfittedItems { get; init; }
	public bool FindSmallestBinOnly { get; init; }
	
	public object ConvertToLogObject()
	{
		List<string> parametersList = new();
		
		if (this.ReportFittedItems)
		{
			parametersList.Add("ReportFittedItems");
		}
		
		if (this.ReportUnfittedItems)
		{
			parametersList.Add("ReportUnfittedItems");
		}
		
		if (this.FindSmallestBinOnly)
		{
			parametersList.Add("FindSmallestBinOnly");
		}
		
		return parametersList;
	}
}
