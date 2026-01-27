using Binacle.Lib.Abstractions.Algorithms;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

internal static class AlgorithmFactories
{
	public static TestAlgorithmFactory<IPackingAlgorithm> FFD_v1 = (bin, items)
		=> new Binacle.Lib.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static TestAlgorithmFactory<IPackingAlgorithm> FFD_v2 = (bin, items)
		=> new Binacle.Lib.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static TestAlgorithmFactory<IPackingAlgorithm> WFD_v1 = (bin, items)
		=> new Binacle.Lib.Algorithms.WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static TestAlgorithmFactory<IPackingAlgorithm> WFD_v2 = (bin, items)
		=> new Binacle.Lib.Algorithms.WorstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static TestAlgorithmFactory<IPackingAlgorithm> BFD_v1 = (bin, items)
		=> new Binacle.Lib.Algorithms.BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static TestAlgorithmFactory<IPackingAlgorithm> BFD_v2 = (bin, items)
		=> new Binacle.Lib.Algorithms.BestFitDecreasing_v2<TestBin, TestItem>(bin, items);
}


internal class PackingAlgorithmFamiliesCollection : Dictionary<string, Dictionary<string, TestAlgorithmFactory<IPackingAlgorithm>>>
{
	public PackingAlgorithmFamiliesCollection()
	{
		this.Add("FFD", new Dictionary<string, TestAlgorithmFactory<IPackingAlgorithm>>
		{
			{ "FFD_v1", AlgorithmFactories.FFD_v1 },
			{ "FFD_v2", AlgorithmFactories.FFD_v2 }
		});
		this.Add("WFD", new Dictionary<string, TestAlgorithmFactory<IPackingAlgorithm>>
		{
			{ "WFD_v1", AlgorithmFactories.WFD_v1 },
			{ "WFD_v2", AlgorithmFactories.WFD_v2 }
		});
		this.Add("BFD", new Dictionary<string, TestAlgorithmFactory<IPackingAlgorithm>>
		{
			{ "BFD_v1", AlgorithmFactories.BFD_v1 },
			{ "BFD_v2", AlgorithmFactories.BFD_v2 }
		});
	}
}
