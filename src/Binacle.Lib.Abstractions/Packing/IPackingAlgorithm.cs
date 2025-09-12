using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Abstractions.Algorithms;

public interface IPackingAlgorithm
{
	AlgorithmInfo AlgorithmInfo { get; }
	
	PackingResult Execute(IPackingParameters parameters);
}
