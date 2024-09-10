using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Abstractions.Fitting;

public interface IFittingAlgorithm
{
	string Name { get; }
	int Version { get; }
	FittingResult Execute(FittingParameters parameters);
}

