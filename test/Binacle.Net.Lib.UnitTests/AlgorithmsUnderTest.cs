using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.UnitTests;

internal static class AlgorithmsUnderTest
{
	public static readonly Dictionary<string, AlgorithmFactory<IFittingAlgorithm>> FittingAlgorithms = new()
	{
		{ "Fitting_FFD_v1", AlgorithmFactories.Fitting_FFD_v1 },
		{ "Fitting_FFD_v2", AlgorithmFactories.Fitting_FFD_v2 },
		{ "Fitting_FFD_v3", AlgorithmFactories.Fitting_FFD_v3 },
		{ "Fitting_WFD_v1", AlgorithmFactories.Fitting_WFD_v1 },
		{ "Fitting_BFD_v1", AlgorithmFactories.Fitting_BFD_v1 }
	};
	
	public static readonly Dictionary<string, AlgorithmFactory<IPackingAlgorithm>> PackingAlgorithms = new()
	{
		{ "Packing_FFD_v1", AlgorithmFactories.Packing_FFD_v1 },
		{ "Packing_FFD_v2", AlgorithmFactories.Packing_FFD_v2 },
		{ "Packing_WFD_v1", AlgorithmFactories.Packing_WFD_v1 },
		{ "Packing_BFD_v1", AlgorithmFactories.Packing_BFD_v1 }
	};
}
