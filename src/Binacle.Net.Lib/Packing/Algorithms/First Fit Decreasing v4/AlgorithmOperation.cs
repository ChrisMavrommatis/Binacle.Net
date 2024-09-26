using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Packing.Algorithms;

internal partial class FirstFitDecreasing_v4<TBin, TItem>
{
	public PackingResult Execute(PackingParameters parameters)
	{
		var resultBuilder = PackingResultBuilder<Bin, Item>.Create(this.bin, this.totalItemCount, this.totalItemsVolume);

		if (parameters.OptInToEarlyFails)
		{
			if (this.totalItemsVolume > this.bin.Volume)
			{
				return resultBuilder
					.WithForcedStatus(PackingResultStatus.EarlyFail_ContainerVolumeExceeded)
					.WithUnpackedItems(this.items[..this.totalItemCount])
					.Build(parameters);
			}

			var itemsNotFittingDueToLongestDimension = this.items[..this.totalItemCount].Where(x => x.LongestDimension > this.bin.LongestDimension);
			if (itemsNotFittingDueToLongestDimension.Any())
			{
				return resultBuilder
					.WithForcedStatus(PackingResultStatus.EarlyFail_ContainerDimensionExceeded)
					.WithUnpackedItems(itemsNotFittingDueToLongestDimension)
					.Build(parameters);
			}
		}

		List<SpaceVolume> availableSpace = [new SpaceVolume(this.bin, Coordinates.Zero)];


		Array.Sort(this.items, (x, y) =>
		{
			if (x is null && y is null)
				return 0;

			if(x is null)
				return 1;

			if(y is null)
				return -1;

			return y.Volume.CompareTo(x.Volume);
		});


		
		for (var i = 0; i < this.totalItemCount; i++)
		{
			var item = this.items[i];

			for (var orientation = 0; orientation < Item.TotalOrientations; orientation++)
			{
				var availableSpaceQuadrant = this.FindAvailableSpace(item, availableSpace);
				if (availableSpaceQuadrant is not null)
				{
					this.Pack(item, availableSpaceQuadrant, availableSpace);
					break;
				}
				item.Rotate();
			}

			// TODO: Optional to stop algorithm from continuing to try to pack all the items it can
		}

		return resultBuilder
			.WithPackedItems(this.items[..this.totalItemCount].Where(x => x.IsPacked))
			.WithUnpackedItems(this.items[..this.totalItemCount].Where(x => !x.IsPacked))
			.Build(parameters);
	}

	private SpaceVolume? FindAvailableSpace(Item orientation, List<SpaceVolume> availableSpace)
	{
		foreach (var space in availableSpace)
		{
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
