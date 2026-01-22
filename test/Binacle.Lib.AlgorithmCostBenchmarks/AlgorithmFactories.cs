using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Algorithms;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.AlgorithmCostBenchmarks;

// internal static class AlgorithmFactories
// {
// 	public static AlgorithmFactory<IPackingAlgorithm> FFD_v1 = (bin, items)
// 		=> new FirstFitDecreasing_v1<TestBin, TestItem>(bin, items);
//
// 	public static AlgorithmFactory<IPackingAlgorithm> FFD_v2 = (bin, items)
// 		=> new FirstFitDecreasing_v2<TestBin, TestItem>(bin, items);
//
//
// 	public static AlgorithmFactory<IPackingAlgorithm> WFD_v1 = (bin, items)
// 		=> new WorstFitDecreasing_v1<TestBin, TestItem>(bin, items);
//
// 	public static AlgorithmFactory<IPackingAlgorithm> WFD_v2 = (bin, items)
// 		=> new WorstFitDecreasing_v2<TestBin, TestItem>(bin, items);
//
//
// 	public static AlgorithmFactory<IPackingAlgorithm> BFD_v1 = (bin, items)
// 		=> new BestFitDecreasing_v1<TestBin, TestItem>(bin, items);
//
// 	public static AlgorithmFactory<IPackingAlgorithm> BFD_v2 = (bin, items)
// 		=> new BestFitDecreasing_v2<TestBin, TestItem>(bin, items);
// }

public class V1_AlgorithmFactory : IAlgorithmFactory
{
	public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = (IPackingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Algorithms.FirstFitDecreasing_v1<TBin, TItem>(bin, items),
			Algorithm.WorstFitDecreasing => new Algorithms.WorstFitDecreasing_v1<TBin, TItem>(bin, items),
			Algorithm.BestFitDecreasing => new Algorithms.BestFitDecreasing_v1<TBin, TItem>(bin, items),
			_ => throw new NotSupportedException($"No Packing Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}
}
public class V2_AlgorithmFactory : IAlgorithmFactory
{
	public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = (IPackingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Algorithms.FirstFitDecreasing_v1<TBin, TItem>(bin, items),
			Algorithm.WorstFitDecreasing => new Algorithms.WorstFitDecreasing_v1<TBin, TItem>(bin, items),
			Algorithm.BestFitDecreasing => new Algorithms.BestFitDecreasing_v1<TBin, TItem>(bin, items),
			_ => throw new NotSupportedException($"No Packing Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}
}
