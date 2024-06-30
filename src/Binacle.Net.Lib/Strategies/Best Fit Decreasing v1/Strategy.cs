using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib.Strategies;

internal sealed class BestFitDecreasing_v1 :
	IBinFittingStrategy,
	IBinFittingStrategyWithBins,
	IBinFittingStrategyWithBinsAndItems,
	IBinFittingOperation

{
	public IBinFittingOperation Build()
	{
		throw new NotImplementedException();
	}

	public BinFittingOperationResult Execute()
	{
		throw new NotImplementedException();
	}

	IBinFittingStrategyWithBinsAndItems IBinFittingStrategyWithBins.AndItems<TItem>(IEnumerable<TItem> items)
	{
		throw new NotImplementedException();
	}

	IBinFittingStrategyWithBins IBinFittingStrategy.WithBins<TBin>(IEnumerable<TBin> bins)
	{
		throw new NotImplementedException();
	}
}
