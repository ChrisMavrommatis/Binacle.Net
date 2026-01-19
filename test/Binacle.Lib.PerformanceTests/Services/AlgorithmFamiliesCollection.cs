using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.PerformanceTests.Services;

internal class AlgorithmFamiliesCollection : Dictionary<string, Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>>
{
	public AlgorithmFamiliesCollection()
	{
		this.Add("FFD", new Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>
		{
			{ "FFD_v1", AlgorithmFactories.FFD_v1 },
			{ "FFD_v2", AlgorithmFactories.FFD_v2 }
		});
		this.Add("WFD", new Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>
		{
			{ "WFD_v1", AlgorithmFactories.WFD_v1 },
			{ "WFD_v2", AlgorithmFactories.WFD_v2 }
		});
		this.Add("BFD", new Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>
		{
			{ "BFD_v1", AlgorithmFactories.BFD_v1 },
			{ "BFD_v2", AlgorithmFactories.BFD_v2 }
		});
	}
}
