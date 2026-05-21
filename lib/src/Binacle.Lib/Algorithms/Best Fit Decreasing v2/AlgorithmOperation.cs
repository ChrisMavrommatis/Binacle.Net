using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.ExtensionMethods;
using Binacle.Lib.Models;

namespace Binacle.Lib.Algorithms;

internal partial class BestFitDecreasing_v2<TBin, TItem>
{
	public OperationResult Execute(IOperationParameters parameters)
	{
		var resultBuilder = this.CreateResultBuilder<Bin, Item>(
			this.bin,
			this.items.Length,
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

		Array.Sort(this.items, (x, y) => y.Volume.CompareTo(x.Volume));

		for (var i = 0; i < this.items.Length; i++)
		{
			var item = this.items[i];
			var packed = false;
			
			for (var orientation = 0; orientation < Item.TotalOrientations; orientation++)
			{
				var availableSpaceQuadrant = this.FindAvailableSpace(item, availableSpace);
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
			.WithPackedItems(this.items.Where(x => x.IsPacked))
			.WithUnpackedItems(this.items.Where(x => !x.IsPacked))
			.Complete();
	}

	private SpaceVolume? FindAvailableSpace(Item orientation, List<SpaceVolume> availableSpace)
	{
		availableSpace.Sort((x, y) => x.Volume.CompareTo(y.Volume));
		for (var i = 0; i < availableSpace.Count; i++)
		{
			var space = availableSpace[i];

			if (space.Dimensions.Length >= orientation.Length && space.Dimensions.Width >= orientation.Width && space.Dimensions.Height >= orientation.Height)
				return space;
		}

		return null;
	}

	private void Pack(Item item, SpaceVolume spaceQuadrant, List<SpaceVolume> availableSpace)
	{
		item.Pack(spaceQuadrant);
		var newAvailableSpaceQuadrants = this.SplitSpaceQuadrant(spaceQuadrant, item);
		availableSpace.Remove(spaceQuadrant);
		if (newAvailableSpaceQuadrants.Length > 0)
		{
			availableSpace.AddRange(newAvailableSpaceQuadrants);
		}

	}

	private SpaceVolume[] SplitSpaceQuadrant(SpaceVolume spaceQuadrant, Item orientation)
	{

		var remainingLength = (ushort)spaceQuadrant.Dimensions.Length - orientation.Length;
		var remainingWidth = (ushort)spaceQuadrant.Dimensions.Width - orientation.Width;
		var remainingHeight = (ushort)spaceQuadrant.Dimensions.Height - orientation.Height;

		ushort newSpaces = 0;


		if (remainingLength > 0)
			newSpaces++;

		if (remainingWidth > 0)
			newSpaces++;

		if (remainingHeight > 0)
			newSpaces++;

		var newAvailableSpaces = new SpaceVolume[newSpaces];


		var offsetX = (ushort)spaceQuadrant.Coordinates.X + orientation.Length;
		var offsetY = (ushort)spaceQuadrant.Coordinates.Y + orientation.Width;
		var offsetZ = (ushort)spaceQuadrant.Coordinates.Z + orientation.Height;

		if (remainingHeight > 0)
			newAvailableSpaces[--newSpaces] = new SpaceVolume(
				new Dimensions(orientation.Length, orientation.Width, remainingHeight),
				new Coordinates(spaceQuadrant.Coordinates.X, spaceQuadrant.Coordinates.Y, offsetZ)
			);


		if (remainingWidth > 0)
			newAvailableSpaces[--newSpaces] = new SpaceVolume(
				new Dimensions(orientation.Length, remainingWidth, spaceQuadrant.Dimensions.Height),
				new Coordinates(spaceQuadrant.Coordinates.X, offsetY, spaceQuadrant.Coordinates.Z)
			);

		if (remainingLength > 0)
			newAvailableSpaces[--newSpaces] = new SpaceVolume(
				new Dimensions(remainingLength, spaceQuadrant.Dimensions.Width, spaceQuadrant.Dimensions.Height),
				new Coordinates(offsetX, spaceQuadrant.Coordinates.Y, spaceQuadrant.Coordinates.Z)
			);


		return newAvailableSpaces;

	}
}
