﻿using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Packing.Algorithms;

internal partial class FirstFitDecreasing_v1<TBin, TItem>
{
	public PackingResult Execute(PackingParameters parameters)
	{
		var resultBuilder = PackingResultBuilder<Bin, Item>.Create(this.bin, this.items.Count, this.totalItemsVolume);

		if (parameters.OptInToEarlyFails)
		{
			if (this.totalItemsVolume > this.bin.Volume)
			{
				return resultBuilder
					.WithForcedStatus(PackingResultStatus.EarlyFail_ContainerVolumeExceeded)
					.WithUnpackedItems(this.items)
					.Build(parameters);
			}

			var itemsNotFittingDueToLongestDimension = this.items.Where(x => x.LongestDimension > this.bin.LongestDimension);
			if (itemsNotFittingDueToLongestDimension.Any())
			{
				return resultBuilder
					.WithForcedStatus(PackingResultStatus.EarlyFail_ContainerDimensionExceeded)
					.WithUnpackedItems(itemsNotFittingDueToLongestDimension)
					.Build(parameters);
			}
		}

		List<SpaceVolume> availableSpace = [new SpaceVolume(this.bin, Coordinates.Zero)];

		var orderedItems = this.items.OrderByDescending(x => x.Volume);
		foreach (var item in orderedItems)
		{
			for (var i = 0; i < Item.TotalOrientations; i++)
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
			.WithPackedItems(orderedItems.Where(x => x.IsPacked))
			.WithUnpackedItems(orderedItems.Where(x => !x.IsPacked))
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