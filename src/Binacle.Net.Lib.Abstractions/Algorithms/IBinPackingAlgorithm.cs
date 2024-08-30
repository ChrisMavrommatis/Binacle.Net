namespace Binacle.Net.Lib.Abstractions.Algorithms;

public interface IBinPackingAlgorithm
{
	string AlgorithmName { get; }
	int Version { get; }
	BinPackingResult Execute();
}
