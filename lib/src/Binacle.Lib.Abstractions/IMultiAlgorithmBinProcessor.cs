using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Abstractions;

public interface IMultiAlgorithmBinProcessor
{
	// Cancellation is honoured between bins, not inside one. See IBinProcessor.
	public IDictionary<string, OperationResult> Process<TBin, TItem>(
		IList<TBin> bins,
		IList<TItem> items,
		IOperationParameters parameters,
		CancellationToken cancellationToken = default
	)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
