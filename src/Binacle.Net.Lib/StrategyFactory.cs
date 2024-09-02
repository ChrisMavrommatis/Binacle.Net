using Binacle.Net.Lib.Abstractions.Fitting;

namespace Binacle.Net.Lib;

public class StrategyFactory
{
	public IFittingAlgorithm Create(Algorithm algorithm)
	{

		var instance = (IFittingAlgorithm)(algorithm switch
		{
			Algorithm.FirstFitDecreasing => new Fitting.Algorithms.FirstFitDecreasing_v1(),
			_ => throw new NotImplementedException($"No Bin Fitting Algorithm exists for {algorithm}")
		});

		return instance;
	}
}

//public class PackingService
//{
//	public BinPackingResult Pack<TBin, TItem>(TBin bin, IList<TItem> items, Algorithm algorithm)
//		where TBin : class, IWithID, IWithReadOnlyDimensions
//		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
//	{
//		var algorithmInstance = algorithm switch
//		{
//			Algorithm.FirstFitDecreasing => new Algorithms.FirstFitDecreasing_v3<TBin, TItem>(bin, items),
//			_ => throw new NotImplementedException($"No algorithm implementation exists for {algorithm}")
//		};

//		return algorithmInstance.Execute();
//	}
//}

//op
