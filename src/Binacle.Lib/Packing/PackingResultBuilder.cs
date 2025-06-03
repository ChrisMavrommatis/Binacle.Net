using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Fitting;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Packing;

internal class PackingResultBuilder<TBin, TItem>
	where TBin : IWithID, IWithReadOnlyVolume
	where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
{
	private readonly AlgorithmInfo algorithmInfo;
	private readonly TBin bin;
	private readonly int totalItems;
	private readonly int totalItemsVolume;
	private PackingResultStatus? forcedStatus;
	private IEnumerable<TItem>? packedItems;
	private IEnumerable<TItem>? unpackedItems;

	internal PackingResultBuilder(AlgorithmInfo algorithmInfo, TBin bin, int totalItems, int totalItemsVolume)
	{
		this.algorithmInfo = algorithmInfo;
		this.bin = bin;
		this.totalItems = totalItems;
		this.totalItemsVolume = totalItemsVolume;
	}

	internal PackingResultBuilder<TBin, TItem> WithForcedStatus(PackingResultStatus status)
	{
		this.forcedStatus = status;
		return this;
	}

	internal PackingResultBuilder<TBin, TItem> WithPackedItems(IEnumerable<TItem> items)
	{
		this.packedItems = items;
		return this;
	}
	internal PackingResultBuilder<TBin, TItem> WithUnpackedItems(IEnumerable<TItem> items)
	{
		this.unpackedItems = items;
		return this;
	}

	internal PackingResult Build(PackingParameters parameters)
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Build Packing Result");
		
		var result = new PackingResult()
		{
			BinID = this.bin.ID,
			Status = this.forcedStatus.HasValue ? this.forcedStatus.Value : PackingResultStatus.Unknown,
			AlgorithmInfo = this.algorithmInfo
		};

		if (!parameters.NeverReportUnpackedItems)
		{
			result.UnpackedItems = new List<ResultItem>();
		}

		var packedItemsCount = 0;
		var unpackedItemsCount = 0;

		var packedItemsVolume = 0;
		var unpackedItemsVolume = 0;

		if(this.unpackedItems is not null)
		{
			foreach (var unpackedItem in this.unpackedItems)
			{
				unpackedItemsCount++;
				unpackedItemsVolume += unpackedItem.Volume;

				if (parameters.NeverReportUnpackedItems)
				{
					continue;
				}

				result.UnpackedItems!.Add(
					new ResultItem(unpackedItem.ID, unpackedItem)
				);
			}
		}

		var reportPackedItems = parameters.ReportPackedItemsOnlyWhenFullyPacked ? unpackedItemsCount == 0 : true;

		if (reportPackedItems)
		{
			result.PackedItems = new List<ResultItem>();
		}

		if(this.packedItems is not null)
		{
			foreach(var packedItem in this.packedItems)
			{
				packedItemsCount++;
				packedItemsVolume += packedItem.Volume;

				if (!reportPackedItems)
				{
					continue;
				}

				result.PackedItems!.Add(
					new ResultItem(packedItem.ID, packedItem, packedItem)
				);
			}
		}

		var totalItemsCount = packedItemsCount + unpackedItemsCount;
		if (totalItemsCount != this.totalItems)
		{
			throw new InvalidOperationException($"The expected total items count is {this.totalItems} but was {totalItemsCount}");
		}

		var totalItemsVolume = packedItemsVolume + unpackedItemsVolume;

		if (totalItemsVolume != this.totalItemsVolume)
		{
			throw new InvalidOperationException($"The expected total items volume is {this.totalItemsVolume} but was {totalItemsVolume}");
		}

		result.PackedBinVolumePercentage = Math.Round((decimal)packedItemsVolume / this.bin.Volume * 100, 2);

		result.PackedItemsVolumePercentage = Math.Round((decimal)packedItemsVolume / totalItemsVolume * 100, 2);

		if(this.forcedStatus.HasValue)
		{ 
			return result; 
		}

		if (packedItemsCount == this.totalItems)
		{
			result.Status = PackingResultStatus.FullyPacked;
		}
		else if (unpackedItemsCount == this.totalItems)
		{
			result.Status = PackingResultStatus.NotPacked;
		}
		else if (packedItemsCount > 0)
		{
			result.Status = PackingResultStatus.PartiallyPacked;
		}

		return result;
	}
}
