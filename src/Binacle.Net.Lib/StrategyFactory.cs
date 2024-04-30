using Binacle.Net.Lib.Abstractions.Strategies;

namespace Binacle.Net.Lib;

public class StrategyFactory
{
	public IBinFittingStrategy Create(BinFittingStrategy strategyType)
	{

		var strategy = (IBinFittingStrategy)(strategyType switch
		{
			BinFittingStrategy.FirstFitDecreasing => new Strategies.FirstFitDecreasing_v1(),
			_ => throw new NotImplementedException($"No Bin Fitting Strategy exists for {strategyType}")
		});

		return strategy;
	}
}
