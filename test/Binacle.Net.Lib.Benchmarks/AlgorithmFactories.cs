using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks;

internal static class AlgorithmFactories
{
	public static Func<TestBin, IEnumerable<TestItem>, IFittingAlgorithm> Fitting_FFD_v1 = (bin, items)
	=> new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

	public static Func<TestBin, IEnumerable<TestItem>, IFittingAlgorithm> Fitting_FFD_v2 = (bin, items)
		=> new Binacle.Net.Lib.Fitting.Algorithms.FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);

	public static Func<TestBin, List<TestItem>, IPackingAlgorithm> Packing_FFD_v1 = (bin, items)
		=> new Binacle.Net.Lib.Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);

}
