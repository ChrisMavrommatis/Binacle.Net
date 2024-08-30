using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Models;

namespace Binacle.Net.Lib;

internal class BinPackingResultBuilder<TBin, TItem>
	where TBin : IWithID, IWithReadOnlyVolume
	where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
{
	private readonly TBin bin;
	private readonly int totalItems;
	private readonly int totalItemsVolume;
	private BinPackingResultStatus? forcedStatus;
	private List<TItem>? packedItems;
	private List<TItem>? unpackedItems;

	internal BinPackingResultBuilder(TBin bin, int totalItems, int totalItemsVolume)
	{
		this.bin = bin;
		this.totalItems = totalItems;
		this.totalItemsVolume = totalItemsVolume;
	}
	internal static BinPackingResultBuilder<TBin, TItem> Create(TBin bin, int totalItems, int totalItemsVolume)
	{
		return new BinPackingResultBuilder<TBin, TItem>(bin, totalItems, totalItemsVolume);
	}

	internal BinPackingResultBuilder<TBin, TItem> WithForcedStatus(BinPackingResultStatus status)
	{
		this.forcedStatus = status;
		return this;
	}

	internal BinPackingResultBuilder<TBin, TItem> WithPackedItems(List<TItem> items)
	{
		this.packedItems = items;
		return this;
	}
	internal BinPackingResultBuilder<TBin, TItem> WithUnpackedItems(List<TItem> items)
	{
		this.unpackedItems = items;
		return this;
	}

	internal BinPackingResult Build()
	{
		var result = new BinPackingResult()
		{
			BinID = this.bin.ID,
			Status = this.forcedStatus.HasValue ? this.forcedStatus.Value : BinPackingResultStatus.Unknown,
		};

		var totalItemsCount = (this.packedItems?.Count ?? 0) + (this.unpackedItems?.Count ?? 0);
		if (totalItemsCount != this.totalItems)
		{
			result.Status = BinPackingResultStatus.Error;
			throw new InvalidOperationException($"The expected total items count is {this.totalItems} but was {totalItemsCount}");
		}

		var packedItemsVolume = this.packedItems?.Sum(x => x.Volume) ?? 0;
		var unpackedItemsVolume = this.unpackedItems?.Sum(x => x.Volume) ?? 0;
		var totalItemsVolume = packedItemsVolume + unpackedItemsVolume;

		if (totalItemsVolume != this.totalItemsVolume)
		{
			result.Status = BinPackingResultStatus.Error;
			throw new InvalidOperationException($"The expected total items volume is {this.totalItemsVolume} but was {totalItemsVolume}");
		}

		result.PackedBinVolumePercentage = Math.Round((decimal)packedItemsVolume / this.bin.Volume * 100, 2);

		result.PackedItemsVolumePercentage = Math.Round((decimal)packedItemsVolume / totalItemsVolume * 100, 2);

		result.PackedItems = this.packedItems?.Select(x => new ResultItem(x.ID, x, x)).ToList();
		result.UnpackedItems = this.unpackedItems?.Select(x => new ResultItem(x.ID, x)).ToList();

		if ((this.packedItems?.Count ?? 0) == this.totalItems)
		{
			result.Status = BinPackingResultStatus.FullyPacked;
		}
		else if ((this.unpackedItems?.Count ?? 0) == this.totalItems)
		{
			result.Status = BinPackingResultStatus.NotPacked;
		}
		else if ((this.packedItems?.Count ?? 0) > 0)
		{
			result.Status = BinPackingResultStatus.PartiallyPacked;
		}

		return result;
	}
}
