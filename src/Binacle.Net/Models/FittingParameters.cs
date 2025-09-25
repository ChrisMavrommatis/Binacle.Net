using Binacle.Lib.Abstractions.Fitting;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Models;

internal class FittingParameters : ILogConvertible, IFittingParameters
{
	public required Algorithm Algorithm { get; init; }
	public bool ReportFittedItems { get; init; }
	public bool ReportUnfittedItems { get; init; }
	
	// This property is deprecated and doesn't work in V3 endpoints
	public bool FindSmallestBinOnly { get; init; }

	public object ConvertToLogObject()
	{
		// Algorithm Is Always added
		var paramsCount = 1;

		if (this.ReportFittedItems)
			paramsCount++;
		if (this.ReportUnfittedItems)
			paramsCount++;
		if (this.FindSmallestBinOnly)
			paramsCount++;
	
		var parameters = new string[paramsCount];

		if (this.FindSmallestBinOnly)
			parameters[--paramsCount] = "FindSmallestBinOnly";

		if (this.ReportUnfittedItems)
			parameters[--paramsCount] = "ReportUnfittedItems";

		if (this.ReportFittedItems)
			parameters[--paramsCount] = "ReportFittedItems";

		parameters[--paramsCount] = this.Algorithm.ToFastString();

		return parameters;
	}
}
