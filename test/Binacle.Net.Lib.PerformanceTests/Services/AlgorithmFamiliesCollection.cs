using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal class AlgorithmFamiliesCollection : Dictionary<string, Dictionary<string, Func<TestBin, List<TestItem>, IPackingAlgorithm>>>
{
	public AlgorithmFamiliesCollection()
	{
		this.Add("FFD", new Dictionary<string, Func<TestBin, List<TestItem>, IPackingAlgorithm>>
		{
			{ "Packing_FFD_v1", AlgorithmFactories.Packing_FFD_v1 },
			{ "Packing_FFD_v2", AlgorithmFactories.Packing_FFD_v2 }
		});
		this.Add("WFD", new Dictionary<string, Func<TestBin, List<TestItem>, IPackingAlgorithm>>
		{
			{ "Packing_WFD_v1", AlgorithmFactories.Packing_WFD_v1 }
		});
		this.Add("BFD", new Dictionary<string, Func<TestBin, List<TestItem>, IPackingAlgorithm>>
		{
			{ "Packing_BFD_v1", AlgorithmFactories.Packing_BFD_v1 }
		});
	}
}

