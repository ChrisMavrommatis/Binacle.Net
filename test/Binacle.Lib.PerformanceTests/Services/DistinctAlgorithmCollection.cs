using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.PerformanceTests.Services;

internal class DistinctAlgorithmCollection : Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>
{
	public DistinctAlgorithmCollection()
	{
		this.Add("FFD", AlgorithmFactories.Packing_FFD_v1);
		this.Add("WFD", AlgorithmFactories.Packing_WFD_v1);
		this.Add("BFD", AlgorithmFactories.Packing_BFD_v1);
	}
}
