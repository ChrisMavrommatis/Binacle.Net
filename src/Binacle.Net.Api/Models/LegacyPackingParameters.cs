
using Binacle.Net.Api.Kernel.Models;

namespace Binacle.Net.Api.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class LegacyPackingParameters : ILogConvertible
{
	public required bool OptInToEarlyFails { get; init; }
	public required bool ReportPackedItemsOnlyWhenFullyPacked { get; init; }
	public required bool NeverReportUnpackedItems { get; init; }
	public required bool StopAtSmallestBin { get; init; }
	
	public object ConvertToLogObject()
	{
		List<string> parametersList = new();
		
		if (this.OptInToEarlyFails)
		{
			parametersList.Add("OptInToEarlyFails");
		}
		
		if (this.ReportPackedItemsOnlyWhenFullyPacked)
		{
			parametersList.Add("ReportPackedItemsOnlyWhenFullyPacked");
		}
		
		if (this.NeverReportUnpackedItems)
		{
			parametersList.Add("NeverReportUnpackedItems");
		}
		
		if (this.StopAtSmallestBin)
		{
			parametersList.Add("StopAtSmallestBin");
		}
		
		return parametersList;
	}
}
