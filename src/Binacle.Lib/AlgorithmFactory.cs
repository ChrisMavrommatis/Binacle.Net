using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public class AlgorithmFactory : IAlgorithmFactory
{
	private static Algorithm[] _algorithmTypes = Enum.GetValues<Algorithm>();
	

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
	
	public IPackingAlgorithm[] Create<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithms = new IPackingAlgorithm[_algorithmTypes.Length];
		for (var i = 0; i < _algorithmTypes.Length; i++)
		{
			algorithms[i] = this.Create<TBin, TItem>(_algorithmTypes[i], bin, items);
		}
		return algorithms;
		
	}
}
