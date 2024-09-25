using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v4<TBin, TItem>
{
	public FittingResult Execute(FittingParameters parameters)
	{
		var resultBuilder = FittingResultBuilder<Bin, Item>.Create(this.bin, this.items.Length, this.totalItemsVolume);

		if (this.totalItemsVolume > this.bin.Volume)
		{
			return resultBuilder
				.WithUnfittedItems(this.items.Where(x => !x.Fitted))
				.WithReason(FittingFailedResultReason.TotalVolumeExceeded)
				.Build(parameters);
		}

		var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > this.bin.LongestDimension);
		if (itemsNotFittingDueToLongestDimension is not null && itemsNotFittingDueToLongestDimension.Any())
		{
			return resultBuilder
				.WithUnfittedItems(this.items.Where(x => !x.Fitted))
				.WithReason(FittingFailedResultReason.ItemDimensionExceeded)
				.Build(parameters);
		}


		this.availableSpace = [
			new VolumetricItem(bin)
		];


		// List -> Array  v3-v4
		// 10  items 3.39 -> 2.34   KB
		// 70  items 15.57 -> 10.36 KB
		// 130 items 27.2 -> 17.8 KB
		// 192 items 38.02 -> 24.61 KB
		// 202 Items 13.01 -> 12.95 KB

		//foreach (var item in this.items.OrderByDescending(x => x.Volume))
		//{
		//	if (!this.TryFit(item))
		//		break;
		//}


		// sort by ascending volume without linq
		//Array.Sort(this.items, (x, y) => x.Volume.CompareTo(y.Volume));

		// sort by descending volume without linq
		Array.Sort(this.items, (x, y) => y.Volume.CompareTo(x.Volume));


		for (var i = 0; i < this.items.Length; i++)
		{
			var item = this.items[i];

			if (!this.TryFit(item))
				break;
		}

		
		return resultBuilder
			.WithFittedItems(this.items.Where(x => x.Fitted))
			.WithUnfittedItems(this.items.Where(x => !x.Fitted))
			.Build(parameters);
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

	private VolumetricItem? FindAvailableSpace(Item orientation)
	{
		for (var i = 0; i < this.availableSpace.Count; i++)
		{
			var space = this.availableSpace[i];

			if (space.Volume < orientation.Volume)
				continue;

			if (space.Length >= orientation.Length && space.Width >= orientation.Width && space.Height >= orientation.Height)
				return space;
		}

		return null;
	}

	private void Fit(VolumetricItem spaceQuadrant, Item item)
	{
		var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, item);
		this.availableSpace.Remove(spaceQuadrant);
		if (newAvailableSpaces.Length > 0)
		{
			this.availableSpace.AddRange(newAvailableSpaces);
		}
		item.Fitted = true;
	}

	private VolumetricItem[] SplitSpaceQuadrant(VolumetricItem spaceQuadrant, Item orientation)
	{
		// List -> Array  v3-v4
		// 10  items 3.39 -> 2.8   KB
		// 70  items 15.57 -> 11.76 KB
		// 130 items 27.2 -> 20.13 KB
		// 192 items 38.02 -> 27.91 KB
		// 202 Items 13.01 -> 12.95 KB

		var remainingLength = (ushort)(spaceQuadrant.Length - orientation.Length);
		var remainingWidth = (ushort)(spaceQuadrant.Width - orientation.Width);
		var remainingHeight = (ushort)(spaceQuadrant.Height - orientation.Height);

		if (remainingLength == 0 && remainingWidth == 0 && remainingHeight == 0)
			return Array.Empty<VolumetricItem>();

		ushort newSpaces = 0;
		if (remainingLength > 0)
			newSpaces++;

		if (remainingWidth > 0)
			newSpaces++;

		if (remainingHeight > 0)
			newSpaces++;

		var newAvailableSpaces = new VolumetricItem[newSpaces];

		//if (remainingLength > 0)
		//	newAvailableSpaces.Add(new VolumetricItem(remainingLength, spaceQuadrant.Width, spaceQuadrant.Height));

		//if (remainingWidth > 0)
		//	newAvailableSpaces.Add(new VolumetricItem(orientation.Length, remainingWidth, spaceQuadrant.Height));

		//if (remainingHeight > 0)
		//	newAvailableSpaces.Add(new VolumetricItem(orientation.Length, orientation.Width, remainingHeight));


		// Reverse so we keep the same order as above
		if (remainingHeight > 0)
			newAvailableSpaces[--newSpaces] = new VolumetricItem(orientation.Length, orientation.Width, remainingHeight);

		if (remainingWidth > 0)
			newAvailableSpaces[--newSpaces] = new VolumetricItem(orientation.Length, remainingWidth, spaceQuadrant.Height);

		if (remainingLength > 0)
			newAvailableSpaces[--newSpaces] = new VolumetricItem(remainingLength, spaceQuadrant.Width, spaceQuadrant.Height);

		return newAvailableSpaces;
	}
}
