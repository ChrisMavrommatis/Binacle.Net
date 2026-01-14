using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.ExtensionMethods;
using Binacle.Lib.Models;

namespace Binacle.Lib.Algorithms;

internal partial class WorstFitDecreasing_v1<TBin, TItem>
{
	public OperationResult Execute(IOperationParameters parameters)
	{
		var resultBuilder = this.CreateResultBuilder<Bin, Item>(
			this.bin, 
			this.items.Count,
			this.totalItemsVolume,
			parameters
		);

		if (parameters.Operation == AlgorithmOperation.Fitting)
		{
			if (this.totalItemsVolume > this.bin.Volume)
			{
				return resultBuilder
					.WithUnpackedItems(this.items.Where(x => !x.IsPacked))
					.EarlyExit(EarlyExitReason.ContainerVolumeExceeded);
			}

			var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > this.bin.LongestDimension);
			if (itemsNotFittingDueToLongestDimension.Any())
			{
				return resultBuilder
					.WithUnpackedItems(this.items.Where(x => !x.IsPacked))
					.EarlyExit(EarlyExitReason.ContainerDimensionExceeded);
			}
		}

		List<SpaceVolume> availableSpace = [new SpaceVolume(this.bin, Coordinates.Zero)];

		var orderedItems = this.items.OrderByDescending(x => x.Volume);
		foreach (var item in orderedItems)
		{
			var packed = false;
			for (var i = 0; i < Item.TotalOrientations; i++)
			{
				var availableSpaceQuadrant = this.FindWorstAvailableSpace(item, availableSpace);
				if (availableSpaceQuadrant is not null)
				{
					this.Pack(item, availableSpaceQuadrant, availableSpace);
					packed = true;
					break;
				}
				item.Rotate();
			}
			if (!packed && parameters.Operation == AlgorithmOperation.Fitting)
			{
				break;
			}
		}

		return resultBuilder
			.WithPackedItems(orderedItems.Where(x => x.IsPacked))
			.WithUnpackedItems(orderedItems.Where(x => !x.IsPacked))
			.Complete();
	}

	private SpaceVolume? FindWorstAvailableSpace(Item orientation, List<SpaceVolume> availableSpace)
	{
		return availableSpace
			.OrderByDescending(x => x.Volume)
			.FirstOrDefault(space => 
				space.Dimensions.Length >= orientation.Length && 
				space.Dimensions.Width >= orientation.Width && 
				space.Dimensions.Height >= orientation.Height
			);
	}

	private void Pack(Item item, SpaceVolume spaceQuadrant, List<SpaceVolume> availableSpace)
	{
		item.Pack(spaceQuadrant);
		var newAvailableSpaceQuadrants = this.SplitSpaceQuadrant(spaceQuadrant, item);
		availableSpace.Remove(spaceQuadrant);
		if (newAvailableSpaceQuadrants.Count > 0)
		{
			availableSpace.AddRange(newAvailableSpaceQuadrants);
		}
		
	}

	private List<SpaceVolume> SplitSpaceQuadrant(SpaceVolume spaceQuadrant, Item orientation)
	{
		var newAvailableSpaces = new List<SpaceVolume>();

		var remainingLength = spaceQuadrant.Dimensions.Length - orientation.Length;
		var remainingWidth = spaceQuadrant.Dimensions.Width - orientation.Width;
		var remainingHeight = spaceQuadrant.Dimensions.Height - orientation.Height;

		var offsetX = spaceQuadrant.Coordinates.X + orientation.Length;
		var offsetY = spaceQuadrant.Coordinates.Y + orientation.Width;
		var offsetZ = spaceQuadrant.Coordinates.Z + orientation.Height;

		if (remainingLength > 0)
			newAvailableSpaces.Add(new SpaceVolume(
				new Dimensions(remainingLength, spaceQuadrant.Dimensions.Width, spaceQuadrant.Dimensions.Height),
				new Coordinates(offsetX, spaceQuadrant.Coordinates.Y, spaceQuadrant.Coordinates.Z)
			));

		if (remainingWidth > 0)
			newAvailableSpaces.Add(new SpaceVolume(
				new Dimensions(orientation.Length, remainingWidth, spaceQuadrant.Dimensions.Height),
				new Coordinates(spaceQuadrant.Coordinates.X, offsetY, spaceQuadrant.Coordinates.Z)
			));

		if (remainingHeight > 0)
			newAvailableSpaces.Add(new SpaceVolume(
				new Dimensions(orientation.Length, orientation.Width, remainingHeight),
				new Coordinates(spaceQuadrant.Coordinates.X, spaceQuadrant.Coordinates.Y, offsetZ)
			));

		return newAvailableSpaces;
	}
}
