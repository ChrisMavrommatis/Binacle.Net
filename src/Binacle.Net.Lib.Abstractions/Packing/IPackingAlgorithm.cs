using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Abstractions.Algorithms;

public interface IPackingAlgorithm
{
	string AlgorithmName { get; }
	int Version { get; }
	PackingResult Execute();
}
