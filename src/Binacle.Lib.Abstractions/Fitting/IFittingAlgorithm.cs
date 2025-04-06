using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Abstractions.Fitting;

public interface IFittingAlgorithm
{
	string Name { get; }
	int Version { get; }
	FittingResult Execute(FittingParameters parameters);
}

