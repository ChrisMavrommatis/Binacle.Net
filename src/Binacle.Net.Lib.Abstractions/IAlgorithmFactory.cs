using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Abstractions;

public interface IAlgorithmFactory
{
	IFittingAlgorithm CreateFitting<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;

	public IPackingAlgorithm CreatePacking<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
