using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Abstractions.Fitting;

public interface IFittingAlgorithm
{
	IFittingAlgorithmWithBins WithBins<TBin>(IEnumerable<TBin> bins)
		 where TBin : class, IWithID, IWithReadOnlyDimensions;
}

public interface IFittingAlgorithmWithBins : IFittingAlgorithm
{
	IFittingAlgorithmWithBinsAndItems AndItems<TItem>(IEnumerable<TItem> items)
		 where TItem : class, IWithID, IWithReadOnlyDimensions;

	//IFittingAlgorithmWithBinsAndItems AndItemsWithQuantity<TItem>(IEnumerable<TItem> items)
	//	 where TItem : class, IWithID, IWithReadOnlyDimensions<int>, IWithQuantity<int>;
}

public interface IFittingAlgorithmWithBinsAndItems : IFittingAlgorithm
{
	IFittingAlgorithmOperation Build();
}

public interface IFittingAlgorithmOperation
{
	FittingResult Execute();
}
