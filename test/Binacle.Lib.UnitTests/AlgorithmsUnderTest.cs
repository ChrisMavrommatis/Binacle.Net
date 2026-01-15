using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

internal static class AlgorithmsUnderTest
{
	public static readonly Dictionary<string, AlgorithmFactory<IPackingAlgorithm>> All = new()
	{
		{ "FFD_v1", AlgorithmFactories.FFD_v1 },
		{ "FFD_v2", AlgorithmFactories.FFD_v2 },
		{ "WFD_v1", AlgorithmFactories.WFD_v1 },
		{ "WFD_v2", AlgorithmFactories.WFD_v2 },
		{ "BFD_v1", AlgorithmFactories.BFD_v1 },
		{ "BFD_v2", AlgorithmFactories.BFD_v2 }
	};
}
