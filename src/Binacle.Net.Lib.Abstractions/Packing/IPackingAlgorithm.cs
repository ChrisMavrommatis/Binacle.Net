using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Abstractions.Algorithms;

public interface IPackingAlgorithm
{
	string Name { get; }
	int Version { get; }
	PackingResult Execute(PackingParameters parameters);
}
