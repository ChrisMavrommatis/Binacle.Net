using Binacle.Net.Lib.Abstractions;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib;

public class StrategyFactory
{
	public IBinFittingStrategy Create(Algorithm algorithm)
	{

		var strategy = (IBinFittingStrategy)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Strategies.FirstFitDecreasing_v1(),
			_ => throw new NotImplementedException($"No Bin Fitting Strategy exists for {algorithm}")
		});

		return strategy;
	}
}

public class BinPackingService
{
	public BinPackingResult Pack<TBin, TItem>(TBin bin, IList<TItem> items, Algorithm algorithm)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		var algorithmInstance = algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Algorithms.FirstFitDecreasing_v3<TBin, TItem>(bin, items),
			_ => throw new NotImplementedException($"No algorithm implementation exists for {algorithm}")
		};

		return algorithmInstance.Execute();
	}
}
