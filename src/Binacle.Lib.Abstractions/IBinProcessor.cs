using Binacle.Lib.Abstractions.Models;
using Binacle.Lib;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Abstractions;

public interface IBinProcessor
{
	public IDictionary<string, FittingResult> ProcessFitting<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		FittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
	
	public IDictionary<string, FittingResult[]> ProcessFitting<TBin, TItem>(
		IList<TBin> bins,
		IList<TItem> items,
		FittingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;


	public IDictionary<string, PackingResult> ProcessPacking<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
	
	public IDictionary<string, PackingResult[]> ProcessPacking<TBin, TItem>(
		IList<TBin> bins,
		IList<TItem> items,
		PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
