using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib.Fitting.Models;

public class FittingResult
{
	internal FittingResult()
	{
	}

	public FittingResultStatus Status { get; internal set; }
	public FittingFailedResultReason? Reason { get; internal set; }
	public Bin FoundBin { get; internal set; }
	public List<Bin> FittedItems { get; internal set; }
	public List<Bin> NotFittedItems { get; internal set; }

	public static FittingResult CreateFailedResult<TItem>(
		FittingFailedResultReason? reason = null,
		IEnumerable<TItem>? fittedItems = null,
		IEnumerable<TItem>? notFittedItems = null
		)
		where TItem: IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
	{
		return new FittingResult()
		{
			Status = FittingResultStatus.Fail,
			Reason = reason.HasValue ? reason.Value : FittingFailedResultReason.Unspecified,
			FittedItems = fittedItems?.Any() ?? false ? fittedItems.Select(x => new Bin(x.ID, x)).ToList() : Enumerable.Empty<Bin>().ToList(),
			NotFittedItems = notFittedItems?.Any() ?? false ? notFittedItems.Select(x => new Bin(x.ID, x)).ToList() : Enumerable.Empty<Bin>().ToList(),
		};
	}

	public static FittingResult CreateSuccessfulResult<TBin, TItem>(
		TBin foundBin,
		IEnumerable<TItem> fittedItems
		)
		where TBin : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
		where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
	{
		if (foundBin == null)
			throw new ArgumentNullException(nameof(foundBin));

		if (!(fittedItems?.Any() ?? false))
			throw new ArgumentNullException(nameof(fittedItems));


		return new FittingResult()
		{
			Status = FittingResultStatus.Success,
			FoundBin = new Bin(foundBin.ID, foundBin),
			FittedItems = fittedItems?.Any() ?? false ? fittedItems.Select(x => new Bin(x.ID, x)).ToList() : Enumerable.Empty<Bin>().ToList(),
		};
	}
}
