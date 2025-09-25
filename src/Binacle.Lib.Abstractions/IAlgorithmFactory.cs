using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib;

namespace Binacle.Lib.Abstractions;

public interface IAlgorithmFactory
{
	IFittingAlgorithm CreateFitting<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
	
	IFittingAlgorithm[] CreateFitting<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;

	public IPackingAlgorithm CreatePacking<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
	
	public IPackingAlgorithm[] CreatePacking<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}

