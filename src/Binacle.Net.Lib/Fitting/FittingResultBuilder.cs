using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Fitting;

internal class FittingResultBuilder<TBin, TItem>
	where TBin : IWithID, IWithReadOnlyVolume
	where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
{
	private readonly TBin bin;
	private readonly int totalItems;
	private readonly int totalItemsVolume;

	private FittingFailedResultReason? reason;

	private IEnumerable<TItem>? fittedItems;
	private IEnumerable<TItem>? unfittedItems;
	internal FittingResultBuilder(TBin bin, int totalItems, int totalItemsVolume)
	{
		this.bin = bin;
		this.totalItems = totalItems;
		this.totalItemsVolume = totalItemsVolume;
	}

	internal static FittingResultBuilder<TBin, TItem> Create(TBin bin, int totalItems, int totalItemsVolume)
	{
		return new FittingResultBuilder<TBin, TItem>(bin, totalItems, totalItemsVolume);
	}

	internal FittingResultBuilder<TBin, TItem> WithFittedItems(IEnumerable<TItem> items)
	{
		this.fittedItems = items;
		return this;
	}
	internal FittingResultBuilder<TBin, TItem> WithUnfittedItems(IEnumerable<TItem> items)
	{
		this.unfittedItems = items;
		return this;
	}

	internal FittingResultBuilder<TBin, TItem> WithReason(FittingFailedResultReason reason)
	{
		this.reason = reason;
		return this;
	}
	internal FittingResult Build(FittingParameters parameters)
	{
		var result = new FittingResult()
		{
			BinID = this.bin.ID
		};

		if (parameters.ReportFittedItems)
		{
			result.FittedItems = new List<ResultItem>();
		}
		if (parameters.ReportUnfittedItems)
		{
			result.UnfittedItems = new List<ResultItem>();
		}

		var fittedItemsCount = 0;
		var unfittedItemsCount = 0;

		var fittedItemsVolume = 0;
		var unfittedItemsVolume = 0;

		if (this.fittedItems is not null)
		{
			foreach (var fittedItem in this.fittedItems)
			{
				if (parameters.ReportFittedItems)
				{
					result.FittedItems!.Add(new ResultItem(fittedItem.ID, fittedItem, fittedItemsCount));
				}
				fittedItemsCount++;
				fittedItemsVolume += fittedItem.Volume;
			}
		}

		if (this.unfittedItems is not null)
		{
			foreach (var unfittedItem in this.unfittedItems)
			{
				if (parameters.ReportUnfittedItems)
				{
					result.UnfittedItems!.Add(new ResultItem(unfittedItem.ID, unfittedItem, unfittedItemsCount));
				}
				unfittedItemsCount++;
				unfittedItemsVolume += unfittedItem.Volume;
			}
		}
		var totalItemsCount = (unfittedItemsCount + fittedItemsCount);
		if (totalItemsCount != this.totalItems)
		{
			throw new InvalidOperationException($"The expected total items count is {this.totalItems} but was {totalItemsCount}");
		}
		var totalItemsVolume = (unfittedItemsVolume + fittedItemsVolume);
		if (totalItemsVolume != this.totalItemsVolume)
		{
			throw new InvalidOperationException($"The expected total items volume is {this.totalItemsVolume} but was {fittedItemsVolume}");
		}

		result.FittedBinVolumePercentage = Math.Round((decimal)fittedItemsVolume / this.bin.Volume * 100, 2);

		result.FittedItemsVolumePercentage = Math.Round((decimal)fittedItemsVolume / totalItemsVolume * 100, 2);

		// TODO Maybe refactor the reason / result
		result.Status = unfittedItemsCount == 0 && fittedItemsCount == this.totalItems ? FittingResultStatus.Success : FittingResultStatus.Fail;
		if (this.reason.HasValue)
		{
			result.Reason = this.reason!.Value;
		}
		else if (result.Status == FittingResultStatus.Fail)
		{
			result.Reason = FittingFailedResultReason.DidNotFit;
		}
		return result;
	}
}
