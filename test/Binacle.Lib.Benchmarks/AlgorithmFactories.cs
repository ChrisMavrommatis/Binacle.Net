using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks;

internal static class AlgorithmFactories
{
	public static AlgorithmFactory<IFittingAlgorithm> Fitting_FFD_v1 = (bin, items)
		=> new Binacle.Lib.Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IFittingAlgorithm> Fitting_FFD_v2 = (bin, items)
		=> new Binacle.Lib.Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IFittingAlgorithm> Fitting_FFD_v3 = (bin, items)
		=> new Binacle.Lib.Fitting.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IFittingAlgorithm> Fitting_WFD_v1 = (bin, items)
		=> new Binacle.Lib.Fitting.Algorithms.WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IFittingAlgorithm> Fitting_BFD_v1 = (bin, items)
		=> new Binacle.Lib.Fitting.Algorithms.BestFitDecreasing_v1<TestBin, TestItem>(bin, items);


	public static AlgorithmFactory<IPackingAlgorithm> Packing_FFD_v1 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static AlgorithmFactory<IPackingAlgorithm> Packing_FFD_v2 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);


	public static AlgorithmFactory<IPackingAlgorithm> Packing_WFD_v1 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IPackingAlgorithm> Packing_WFD_v2 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.WorstFitDecreasing_v2<TestBin, TestItem>(bin, items);


	public static AlgorithmFactory<IPackingAlgorithm> Packing_BFD_v1 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
	
	public static AlgorithmFactory<IPackingAlgorithm> Packing_BFD_v2 = (bin, items)
		=> new Binacle.Lib.Packing.Algorithms.BestFitDecreasing_v2<TestBin, TestItem>(bin, items);
}
