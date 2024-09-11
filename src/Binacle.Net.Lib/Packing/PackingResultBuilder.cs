using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Packing;

internal class PackingResultBuilder<TBin, TItem>
	where TBin : IWithID, IWithReadOnlyVolume
	where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
{
	private readonly TBin bin;
	private readonly int totalItems;
	private readonly int totalItemsVolume;
	private PackingResultStatus? forcedStatus;
	private List<TItem>? packedItems;
	private List<TItem>? unpackedItems;

	internal PackingResultBuilder(TBin bin, int totalItems, int totalItemsVolume)
	{
		this.bin = bin;
		this.totalItems = totalItems;
		this.totalItemsVolume = totalItemsVolume;
	}
	internal static PackingResultBuilder<TBin, TItem> Create(TBin bin, int totalItems, int totalItemsVolume)
	{
		return new PackingResultBuilder<TBin, TItem>(bin, totalItems, totalItemsVolume);
	}

	internal PackingResultBuilder<TBin, TItem> WithForcedStatus(PackingResultStatus status)
	{
		this.forcedStatus = status;
		return this;
	}

	internal PackingResultBuilder<TBin, TItem> WithPackedItems(List<TItem> items)
	{
		this.packedItems = items;
		return this;
	}
	internal PackingResultBuilder<TBin, TItem> WithUnpackedItems(List<TItem> items)
	{
		this.unpackedItems = items;
		return this;
	}

	internal PackingResult Build(PackingParameters parameters)
	{
		var result = new PackingResult()
		{
			BinID = this.bin.ID,
			Status = this.forcedStatus.HasValue ? this.forcedStatus.Value : PackingResultStatus.Unknown,
		};

		var totalItemsCount = (this.packedItems?.Count ?? 0) + (this.unpackedItems?.Count ?? 0);
		if (totalItemsCount != this.totalItems)
		{
			throw new InvalidOperationException($"The expected total items count is {this.totalItems} but was {totalItemsCount}");
		}

		var packedItemsVolume = this.packedItems?.Sum(x => x.Volume) ?? 0;
		var unpackedItemsVolume = this.unpackedItems?.Sum(x => x.Volume) ?? 0;
		var totalItemsVolume = packedItemsVolume + unpackedItemsVolume;

		if (totalItemsVolume != this.totalItemsVolume)
		{
			throw new InvalidOperationException($"The expected total items volume is {this.totalItemsVolume} but was {totalItemsVolume}");
		}

		result.PackedBinVolumePercentage = Math.Round((decimal)packedItemsVolume / this.bin.Volume * 100, 2);

		result.PackedItemsVolumePercentage = Math.Round((decimal)packedItemsVolume / totalItemsVolume * 100, 2);

		result.PackedItems = this.packedItems?.Select(x => new Models.ResultItem(x.ID, x, x)).ToList();
		result.UnpackedItems = this.unpackedItems?.Select(x => new Models.ResultItem(x.ID, x)).ToList();

		if ((this.packedItems?.Count ?? 0) == this.totalItems)
		{
			result.Status = PackingResultStatus.FullyPacked;
		}
		else if ((this.unpackedItems?.Count ?? 0) == this.totalItems)
		{
			result.Status = PackingResultStatus.NotPacked;
		}
		else if ((this.packedItems?.Count ?? 0) > 0)
		{
			result.Status = PackingResultStatus.PartiallyPacked;
		}

		return result;
	}
}
