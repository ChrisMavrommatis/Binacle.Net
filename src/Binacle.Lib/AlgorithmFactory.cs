using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public class AlgorithmFactory : IAlgorithmFactory
{
	public IFittingAlgorithm CreateFitting<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = (IFittingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Fitting.Algorithms.FirstFitDecreasing_v3<TBin, TItem>(bin, items),
			Algorithm.WorstFitDecreasing => new Fitting.Algorithms.WorstFitDecreasing_v1<TBin, TItem>(bin, items),
			Algorithm.BestFitDecreasing => new Fitting.Algorithms.BestFitDecreasing_v1<TBin, TItem>(bin, items),
			_ => throw new NotSupportedException($"No Bin Fitting Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}

	public IPackingAlgorithm CreatePacking<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = (IPackingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Packing.Algorithms.FirstFitDecreasing_v2<TBin, TItem>(bin, items),
			Algorithm.WorstFitDecreasing => new Packing.Algorithms.WorstFitDecreasing_v1<TBin, TItem>(bin, items),
			Algorithm.BestFitDecreasing => new Packing.Algorithms.BestFitDecreasing_v1<TBin, TItem>(bin, items),
			_ => throw new NotSupportedException($"No Packing Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}
}
