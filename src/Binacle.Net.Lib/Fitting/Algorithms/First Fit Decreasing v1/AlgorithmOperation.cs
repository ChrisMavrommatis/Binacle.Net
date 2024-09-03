﻿using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1 :
	IFittingAlgorithmOperation
{
	public FittingResult Execute()
	{
		Bin? foundBin = null;
		int totalItemsToFit = _items.Count();

		var largestBinByVolume = (_bins.OrderByDescending(x => x.Volume).FirstOrDefault())!;
		if (_items.Sum(x => x.Volume) > largestBinByVolume.Volume)
			return FittingResult.CreateFailedResult<Item>(FittingFailedResultReason.TotalVolumeExceeded);

		var itemsNotFittingDueToLongestDimension = _items.Where(x => x.LongestDimension > largestBinByVolume.LongestDimension);
		if (itemsNotFittingDueToLongestDimension.Any())
			return FittingResult.CreateFailedResult(FittingFailedResultReason.ItemDimensionExceeded, itemsNotFittingDueToLongestDimension);

		_bins = _bins.OrderBy(x => x.Volume);
		_items = _items.OrderByDescending(x => x.Volume);

		foreach (var bin in _bins)
		{
			_availableSpace = new List<VolumetricItem>
			{
				new VolumetricItem(bin)
			};

			_fittedItems = new List<Item>();

			foreach (var item in _items)
			{
				if (!this.TryFit(item))
					break;
			}
			if (_fittedItems.Count == totalItemsToFit)
			{
				foundBin = bin;
				break;
			}
		}

		if (foundBin != null)
		{
			return FittingResult.CreateSuccessfulResult(foundBin, this._fittedItems);
		}

		return FittingResult.CreateFailedResult(FittingFailedResultReason.DidNotFit, fittedItems: this._fittedItems);
	}


	private bool TryFit(Item item)
	{
		foreach (var orientation in item.GetOrientations())
		{
			var availableSpaceQuadrant = this.FindAvailableSpace(orientation);
			if (availableSpaceQuadrant != null)
			{
				this.Fit(availableSpaceQuadrant, orientation, item);
				return true;
			}
		}
		return false;
	}

	private VolumetricItem? FindAvailableSpace(VolumetricItem orientation)
	{
		return _availableSpace.FirstOrDefault(x => x.Length >= orientation.Length && x.Width >= orientation.Width && x.Height >= orientation.Height);
	}

	private void Fit(VolumetricItem spaceQuadrant, VolumetricItem orientation, Item item)
	{
		var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, orientation);
		_availableSpace.Remove(spaceQuadrant);
		if (newAvailableSpaces.Any())
		{
			_availableSpace.AddRange(newAvailableSpaces);
		}
		_fittedItems.Add(item);
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
