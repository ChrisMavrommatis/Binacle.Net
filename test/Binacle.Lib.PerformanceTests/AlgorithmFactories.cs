using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.PerformanceTests;

internal static class AlgorithmFactories
{
	public static AlgorithmFactory<IPackingAlgorithm> Packing_FFD_v1 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> Packing_FFD_v2 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);


	public static AlgorithmFactory<IPackingAlgorithm> Packing_WFD_v1 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);


	public static AlgorithmFactory<IPackingAlgorithm> Packing_BFD_v1 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
	public static AlgorithmFactory<IPackingAlgorithm> Packing_BFD_v2 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.BestFitDecreasing_v2<TestBin, TestItem>(bin, items);
}
