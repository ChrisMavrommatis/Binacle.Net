using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Fitting;

internal class FittingResultBuilder<TBin, TItem>
	where TBin : IWithID, IWithReadOnlyVolume
	where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
{
	private readonly AlgorithmInfo algorithmInfo;
	private readonly TBin bin;
	private readonly int totalItems;
	private readonly int totalItemsVolume;

	private FittingFailedResultReason? reason;

	private IEnumerable<TItem>? fittedItems;
	private IEnumerable<TItem>? unfittedItems;
	internal FittingResultBuilder(AlgorithmInfo algorithmInfo, TBin bin, int totalItems, int totalItemsVolume)
	{
		this.algorithmInfo = algorithmInfo;
		this.bin = bin;
		this.totalItems = totalItems;
		this.totalItemsVolume = totalItemsVolume;
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
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Build Fitting Result");
		
		var result = new FittingResult()
		{
			BinID = this.bin.ID,
			AlgorithmInfo = this.algorithmInfo
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
