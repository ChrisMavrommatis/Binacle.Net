using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Strategies;

internal sealed partial class FirstFitDecreasing_v2 :
	IBinFittingOperation
{
	public BinFittingOperationResult Execute()
	{
		Bin? foundBin = null;
		int totalItemsToFit = this.items.Count();
		var totalItemsVolume = this.items.Sum(x => x.Volume);

		var largestBinByVolume = (this.bins.OrderByDescending(x => x.Volume).FirstOrDefault())!;
		if (totalItemsVolume > largestBinByVolume.Volume)
			return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.TotalVolumeExceeded);

		var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > largestBinByVolume.LongestDimension);
		if (itemsNotFittingDueToLongestDimension.Any())
			return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.ItemDimensionExceeded, itemsNotFittingDueToLongestDimension);

		this.bins = this.bins.OrderBy(x => x.Volume);
		this.items = this.items.OrderByDescending(x => x.Volume);

		foreach (var bin in this.bins)
		{
			if (bin.Volume < totalItemsVolume)
			{
				continue;
			}

			this.availableSpace = new List<VolumetricItem>
			{
				new VolumetricItem(bin)
			};

			this.fittedItems = new List<Item>();

			foreach (var item in this.items)
			{
				if (!this.TryFit(item))
					break;
			}
			if (this.fittedItems.Count == totalItemsToFit)
			{
				foundBin = bin;
				break;
			}
		}

		if (foundBin != null)
		{
			return BinFittingOperationResult.CreateSuccessfulResult(foundBin, this.fittedItems);
		}

		return BinFittingOperationResult.CreateFailedResult(BinFitFailedResultReason.DidNotFit, fittedItems: this.fittedItems);
	}

	private bool TryFit(Item item)
	{
		for (var i = 0; i < Item.TotalOrientations; i++)
		{
			var availableSpaceQuadrant = this.FindAvailableSpace(item);
			if (availableSpaceQuadrant != null)
			{
				this.Fit(availableSpaceQuadrant, item);
				return true;
			}
			item.Rotate();
		}
		return false;
	}

	private VolumetricItem? FindAvailableSpace(VolumetricItem orientation)
	{
		foreach (var space in this.availableSpace)
		{
			if (space.Length >= orientation.Length && space.Width >= orientation.Width && space.Height >= orientation.Height)
				return space;
		}

		return null;
	}

	private void Fit(VolumetricItem spaceQuadrant, Item item)
	{
		var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, item);
		this.availableSpace.Remove(spaceQuadrant);
		if (newAvailableSpaces.Any())
		{
			this.availableSpace.AddRange(newAvailableSpaces);
		}
		this.fittedItems.Add(item);
	}

	private List<VolumetricItem> SplitSpaceQuadrant(VolumetricItem spaceQuadrant, VolumetricItem orientation)
	{
		var newAvailableSpaces = new List<VolumetricItem>();

		var remainingLength = (ushort)(spaceQuadrant.Length - orientation.Length);
		var remainingWidth = (ushort)(spaceQuadrant.Width - orientation.Width);
		var remainingHeight = (ushort)(spaceQuadrant.Height - orientation.Height);

		if (remainingLength > 0)
			newAvailableSpaces.Add(new VolumetricItem(remainingLength, spaceQuadrant.Width, spaceQuadrant.Height));

		if (remainingWidth > 0)
			newAvailableSpaces.Add(new VolumetricItem(orientation.Length, remainingWidth, spaceQuadrant.Height));

		if (remainingHeight > 0)
			newAvailableSpaces.Add(new VolumetricItem(orientation.Length, orientation.Width, remainingHeight));

		return newAvailableSpaces;
	}
}
