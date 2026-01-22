using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public class AlgorithmFactory : IAlgorithmFactory
{
	public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = (IPackingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Algorithms.FirstFitDecreasing_v2<TBin, TItem>(bin, items),
			Algorithm.WorstFitDecreasing => new Algorithms.WorstFitDecreasing_v2<TBin, TItem>(bin, items),
			Algorithm.BestFitDecreasing => new Algorithms.BestFitDecreasing_v2<TBin, TItem>(bin, items),
			_ => throw new NotSupportedException($"No Packing Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}
}
