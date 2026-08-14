using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib.Abstractions;

public interface IAlgorithmFactory
{
	public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
