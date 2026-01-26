using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.PerformanceTests;

internal static class AlgorithmFactories
{
	public static AlgorithmFactory<IPackingAlgorithm>[] All =
	[
		FFD_v1,
		FFD_v2,
		WFD_v1,
		WFD_v2,
		BFD_v1,
		BFD_v2
	];
	
	public static AlgorithmFactory<IPackingAlgorithm> FFD_v1 = (bin, items)
		=> new FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> FFD_v2 = (bin, items)
		=> new FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> WFD_v1 = (bin, items)
		=> new WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IPackingAlgorithm> WFD_v2 = (bin, items)
		=> new WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> BFD_v1 = (bin, items)
		=> new BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IPackingAlgorithm> BFD_v2 = (bin, items)
		=> new BestFitDecreasing_v2<TestBin, TestItem>(bin, items);
}
