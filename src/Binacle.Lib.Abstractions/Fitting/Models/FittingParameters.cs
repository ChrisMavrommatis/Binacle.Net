using Binacle.Lib.Abstractions.Fitting;


namespace Binacle.Lib.Fitting.Models;

public class FittingParameters : IFittingParameters
{
	public required bool ReportFittedItems { get; init; }
	public required bool ReportUnfittedItems { get; init; }
}
