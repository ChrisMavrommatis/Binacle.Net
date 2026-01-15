using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

internal static class AlgorithmFactories
{
	public static AlgorithmFactory<IPackingAlgorithm> FFD_v1 = (bin, items)
		=> new Binacle.Lib.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> FFD_v2 = (bin, items)
		=> new Binacle.Lib.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> WFD_v1 = (bin, items)
		=> new Binacle.Lib.Algorithms.WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IPackingAlgorithm> WFD_v2 = (bin, items)
		=> new Binacle.Lib.Algorithms.WorstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> BFD_v1 = (bin, items)
		=> new Binacle.Lib.Algorithms.BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IPackingAlgorithm> BFD_v2 = (bin, items)
		=> new Binacle.Lib.Algorithms.BestFitDecreasing_v2<TestBin, TestItem>(bin, items);
}


internal class PackingAlgorithmFamiliesCollection : Dictionary<string, Dictionary<string, AlgorithmFactory<IPackingAlgorithm>>>
{
	public PackingAlgorithmFamiliesCollection()
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
