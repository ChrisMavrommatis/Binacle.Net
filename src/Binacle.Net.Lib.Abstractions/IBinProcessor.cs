using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Abstractions;

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


	public IDictionary<string, PackingResult> ProcessPacking<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		PackingParameters parameters
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
