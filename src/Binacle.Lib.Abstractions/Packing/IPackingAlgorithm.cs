using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Abstractions.Algorithms;

public interface IPackingAlgorithm
{
	string Name { get; }
	int Version { get; }
	PackingResult Execute(PackingParameters parameters);
}
