using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Models;

public class BinFittingOperationResult
{
    internal BinFittingOperationResult()
    {
    }

    public BinFitResultStatus Status { get; private set; }
    public BinFitFailedResultReason? Reason { get; private set; }
    public IItemWithReadOnlyDimensions<int> FoundBin { get; private set; }
    public List<IItemWithReadOnlyDimensions<int>> FittedItems { get; private set; }
    public List<IItemWithReadOnlyDimensions<int>> NotFittedItems { get; private set; }

    public static BinFittingOperationResult CreateFailedResult(
        BinFitFailedResultReason? reason = null, 
        IEnumerable<IItemWithReadOnlyDimensions<int>>? fittedItems = null,
        IEnumerable<IItemWithReadOnlyDimensions<int>>? notFittedItems = null
        )
    {
        return new BinFittingOperationResult()
        {
            Status = BinFitResultStatus.Fail,
            Reason = reason.HasValue ? reason.Value : BinFitFailedResultReason.Unspecified,
            FittedItems = (fittedItems?.Any() ?? false) ? fittedItems.ToList() : Enumerable.Empty<IItemWithReadOnlyDimensions<int>>().ToList(),
            NotFittedItems = (notFittedItems?.Any() ?? false) ? notFittedItems.ToList() : Enumerable.Empty<IItemWithReadOnlyDimensions<int>>().ToList(),
        };
    }

    public static BinFittingOperationResult CreateSuccessfulResult(
        IItemWithReadOnlyDimensions<int> foundBin, 
        IEnumerable<IItemWithReadOnlyDimensions<int>> fittedItems
        )
    {
        if (foundBin == null)
            throw new ArgumentNullException(nameof(foundBin));

        if (!(fittedItems?.Any() ?? false))
            throw new ArgumentNullException(nameof(fittedItems));


        return new BinFittingOperationResult()
        {
            Status = BinFitResultStatus.Success,
            FoundBin = foundBin,
            FittedItems = (fittedItems?.Any() ?? false) ? fittedItems.ToList() : Enumerable.Empty<IItemWithReadOnlyDimensions<int>>().ToList(),
        };
    }
}
