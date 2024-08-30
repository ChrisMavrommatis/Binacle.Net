using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Strategies;

namespace Binacle.Net.Lib;

public class BinFittingOperationResult
{
	internal BinFittingOperationResult()
	{
	}

	public BinFitResultStatus Status { get; internal set; }
	public BinFitFailedResultReason? Reason { get; internal set; }
	public IItemWithReadOnlyDimensions<int> FoundBin { get; internal set; }
	public List<IItemWithReadOnlyDimensions<int>> FittedItems { get; internal set; }
	public List<IItemWithReadOnlyDimensions<int>> NotFittedItems { get; internal set; }

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
