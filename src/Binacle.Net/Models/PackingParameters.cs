using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Models;

internal class PackingParameters : ILogConvertible, IPackingParameters
{
	public required Algorithm Algorithm { get; init; }
	public required bool OptInToEarlyFails { get; init; }
	public required bool ReportPackedItemsOnlyWhenFullyPacked { get; init; }
	public required bool NeverReportUnpackedItems { get; init; }
	
	// This property is deprecated and doesn't work in V3 endpoints
	public bool StopAtSmallestBin { get; init; }
	
	public object ConvertToLogObject()
	{
		// Algorithm Is Always added
		var paramsCount = 1;

		if (this.OptInToEarlyFails)
			paramsCount++;
		if (this.ReportPackedItemsOnlyWhenFullyPacked)
			paramsCount++;
		if (this.NeverReportUnpackedItems)
			paramsCount++;
		if (this.StopAtSmallestBin)
			paramsCount++;

		var parameters = new string[paramsCount];

		if (this.StopAtSmallestBin)
			parameters[--paramsCount] = "StopAtSmallestBin";
		
		if (this.NeverReportUnpackedItems)
			parameters[--paramsCount] = "NeverReportUnpackedItems";
		
		if (this.ReportPackedItemsOnlyWhenFullyPacked)
			parameters[--paramsCount] = "ReportPackedItemsOnlyWhenFullyPacked";
		
		if (this.OptInToEarlyFails)
			parameters[--paramsCount] = "OptInToEarlyFails";

		parameters[--paramsCount] = this.Algorithm.ToFastString();
		
		return parameters;
	}
	
}
