using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib;

// The IPackingAlgorithm half of the identifier naming. Its AlgorithmInfo sibling lives one layer down in
// Binacle.Packing, which is why this delegates rather than repeating the formatting: an algorithm instance
// carries exactly the Algorithm and Version an AlgorithmInfo holds, so the two names can never drift.
public static class PackingAlgorithmExtensions
{
	public static string GetAlgorithmIdentifierName(this IPackingAlgorithm algorithm)
	{
		return new AlgorithmInfo(algorithm.Algorithm, algorithm.Version).GetAlgorithmIdentifierName();
	}
}
