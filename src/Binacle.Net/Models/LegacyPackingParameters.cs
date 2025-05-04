
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Models;

internal class LegacyPackingParameters : ILogConvertible
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
