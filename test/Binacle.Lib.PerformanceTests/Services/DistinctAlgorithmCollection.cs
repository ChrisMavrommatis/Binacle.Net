using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.PerformanceTests.Services;

internal class DistinctAlgorithmCollection : Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>
{
	public DistinctAlgorithmCollection()
	{
		this.Add("FFD", AlgorithmFactories.FFD_v1);
		this.Add("FFD2", AlgorithmFactories.FFD_v2);
		this.Add("WFD", AlgorithmFactories.WFD_v1);
		this.Add("WFD2", AlgorithmFactories.WFD_v2);
		this.Add("BFD", AlgorithmFactories.BFD_v1);
		this.Add("BFD2", AlgorithmFactories.BFD_v2);
	}
}
