using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib.Abstractions.Strategies;

public interface IBinFittingStrategy
{
	IBinFittingStrategyWithBins WithBins<TBin>(IEnumerable<TBin> bins)
		 where TBin : class, IWithID, IWithReadOnlyDimensions<int>;
}

public interface IBinFittingStrategyWithBins : IBinFittingStrategy
{
	IBinFittingStrategyWithBinsAndItems AndItems<TItem>(IEnumerable<TItem> items)
		 where TItem : class, IWithID, IWithReadOnlyDimensions<int>;

	//IBinFittingStrategyWithBinsAndItems AndItemsWithQuantity<TItem>(IEnumerable<TItem> items)
	//     where TItem : class, IWithID, IWithReadOnlyDimensions<int>, IWithQuantity<int>;
}

public interface IBinFittingStrategyWithBinsAndItems : IBinFittingStrategy
{
	IBinFittingOperation Build();
}

public interface IBinFittingOperation
{
	BinFittingOperationResult Execute();
}
