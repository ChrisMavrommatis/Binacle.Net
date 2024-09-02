using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Models;

public class FittingResult
{
	internal FittingResult()
	{
	}

	public FittingResultStatus Status { get; internal set; }
	public FittingFailedResultReason? Reason { get; internal set; }
	public IItemWithReadOnlyDimensions<int> FoundBin { get; internal set; }
	public List<IItemWithReadOnlyDimensions<int>> FittedItems { get; internal set; }
	public List<IItemWithReadOnlyDimensions<int>> NotFittedItems { get; internal set; }

	public static FittingResult CreateFailedResult(
		FittingFailedResultReason? reason = null,
		IEnumerable<IItemWithReadOnlyDimensions<int>>? fittedItems = null,
		IEnumerable<IItemWithReadOnlyDimensions<int>>? notFittedItems = null
		)
	{
		return new FittingResult()
		{
			Status = FittingResultStatus.Fail,
			Reason = reason.HasValue ? reason.Value : FittingFailedResultReason.Unspecified,
			FittedItems = fittedItems?.Any() ?? false ? fittedItems.ToList() : Enumerable.Empty<IItemWithReadOnlyDimensions<int>>().ToList(),
			NotFittedItems = notFittedItems?.Any() ?? false ? notFittedItems.ToList() : Enumerable.Empty<IItemWithReadOnlyDimensions<int>>().ToList(),
		};
	}

	public static FittingResult CreateSuccessfulResult(
		IItemWithReadOnlyDimensions<int> foundBin,
		IEnumerable<IItemWithReadOnlyDimensions<int>> fittedItems
		)
	{
		if (foundBin == null)
			throw new ArgumentNullException(nameof(foundBin));

		if (!(fittedItems?.Any() ?? false))
			throw new ArgumentNullException(nameof(fittedItems));


		return new FittingResult()
		{
			Status = FittingResultStatus.Success,
			FoundBin = foundBin,
			FittedItems = fittedItems?.Any() ?? false ? fittedItems.ToList() : Enumerable.Empty<IItemWithReadOnlyDimensions<int>>().ToList(),
		};
	}
}
