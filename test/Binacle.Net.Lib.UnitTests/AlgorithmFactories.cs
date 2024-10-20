using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.UnitTests;

internal static class AlgorithmFactories
{
	public static Func<TestBin, List<TestItem>, IFittingAlgorithm> Fitting_FFD_v1 = (bin, items)
		=> new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IFittingAlgorithm> Fitting_FFD_v2 = (bin, items)
		=> new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IFittingAlgorithm> Fitting_FFD_v3 = (bin, items)
		=> new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v3<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IPackingAlgorithm> Packing_FFD_v1 = (bin, items)
		=> new Binacle.Net.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IPackingAlgorithm> Packing_FFD_v2 = (bin, items)
		=> new Binacle.Net.Lib.Packing.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IPackingAlgorithm> Packing_WFD_v1 = (bin, items)
		=> new Binacle.Net.Lib.Packing.Algorithms.WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IPackingAlgorithm> Packing_BFD_v1 = (bin, items)
		=> new Binacle.Net.Lib.Packing.Algorithms.BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
}
