using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Abstractions;

public interface IBinProcessor
{
	// Cancellation is honoured between bins, not inside one. A bin's packing run is short enough
	// (tens of ms) that tearing it apart mid-run costs more than it saves.
	public IDictionary<string, OperationResult> Process<TBin, TItem>(
		Algorithm algorithm,
		IList<TBin> bins,
		IList<TItem> items,
		IOperationParameters parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
