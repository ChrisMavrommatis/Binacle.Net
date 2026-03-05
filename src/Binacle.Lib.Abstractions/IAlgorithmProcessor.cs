using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Abstractions;

public interface IAlgorithmProcessor
{
    public IDictionary<string, OperationResult> Process<TBin, TItem>(
        TBin bin,
        IList<TItem> items,
        IOperationParameters parameters
    )
        where TBin : class, IWithID, IWithReadOnlyDimensions
        where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity;
}
