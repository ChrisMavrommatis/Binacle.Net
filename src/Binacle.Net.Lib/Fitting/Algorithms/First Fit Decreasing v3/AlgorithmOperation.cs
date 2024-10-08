using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v3<TBin, TItem>
{
	public FittingResult Execute(FittingParameters parameters)
	{
		var resultBuilder = FittingResultBuilder<Bin, Item>.Create(this.bin, this.items.Count, this.totalItemsVolume);

		if (this.totalItemsVolume > this.bin.Volume)
		{
			return resultBuilder
				.WithUnfittedItems(this.items.Where(x => !x.Fitted))
				.WithReason(FittingFailedResultReason.TotalVolumeExceeded)
				.Build(parameters);
		}

		var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > this.bin.LongestDimension);
		if (itemsNotFittingDueToLongestDimension.Any())
		{
			return resultBuilder
				.WithUnfittedItems(this.items.Where(x => !x.Fitted))
				.WithReason(FittingFailedResultReason.ItemDimensionExceeded)
				.Build(parameters);
		}


		this.availableSpace = [
			new VolumetricItem(bin)
		];

		foreach (var item in this.items.OrderByDescending(x => x.Volume))
		{
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

			if (space.Length >= orientation.Length && space.Width >= orientation.Width && space.Height >= orientation.Height)
				return space;
		}

		return null;
	}

	private void Fit(VolumetricItem spaceQuadrant, Item item)
	{
		var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, item);
		this.availableSpace.Remove(spaceQuadrant);
		if (newAvailableSpaces.Count > 0)
		{
			this.availableSpace.AddRange(newAvailableSpaces);
		}
		item.Fitted = true;
	}

	private List<VolumetricItem> SplitSpaceQuadrant(VolumetricItem spaceQuadrant, Item orientation)
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
