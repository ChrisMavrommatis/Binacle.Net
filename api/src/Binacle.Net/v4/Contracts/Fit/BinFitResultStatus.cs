using System.ComponentModel;
namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

[Description("Outcome of a fit check.")]
public enum BinFitResultStatus
{
	Unknown = -1,
	Fits = 0,
	DoesNotFit = 1,
	EarlyExit = 2,
}
