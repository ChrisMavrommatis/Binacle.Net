using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib;

public class AlgorithmFactory
{
	public IFittingAlgorithm Create(Algorithm algorithm)
	{

		var algorithmInstance = (IFittingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Fitting.Algorithms.FirstFitDecreasing_v1(),
			_ => throw new NotImplementedException($"No Bin Fitting Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}

	public IPackingAlgorithm CreatePacking<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = (IPackingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Packing.Algorithms.FirstFitDecreasing_v1<TBin, TItem>(bin, items),
			_ => throw new NotImplementedException($"No Packing Algorithm exists for {algorithm}")
		});

		return algorithmInstance;
	}
}
