using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Abstractions.Fitting;

public interface IFittingAlgorithm
{
	AlgorithmInfo AlgorithmInfo { get; }

	FittingResult Execute(FittingParameters parameters);
}

