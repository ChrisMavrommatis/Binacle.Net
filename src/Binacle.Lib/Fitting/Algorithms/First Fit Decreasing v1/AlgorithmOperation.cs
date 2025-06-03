using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1<TBin, TItem> : IFittingAlgorithm
{
	public FittingResult Execute(FittingParameters parameters)
	{
		int totalItemsVolume = _items.Sum(x => x.Volume);

		var resultBuilder = this.CreateResultBuilder<Bin, Item>(_bin, _items.Count, totalItemsVolume);

		if (totalItemsVolume > _bin.Volume)
		{
			return resultBuilder
				.WithUnfittedItems(_items.Where(x => !x.Fitted))
				.WithReason(FittingFailedResultReason.TotalVolumeExceeded)
				.Build(parameters);
		}

		var itemsNotFittingDueToLongestDimension = _items.Where(x => x.LongestDimension > _bin.LongestDimension);
		if (itemsNotFittingDueToLongestDimension.Any())
		{
			return resultBuilder
				.WithUnfittedItems(_items.Where(x => !x.Fitted))
				.WithReason(FittingFailedResultReason.ItemDimensionExceeded)
				.Build(parameters);
		}

		_availableSpace = new List<VolumetricItem>
		{
			new VolumetricItem(_bin)
		};

		foreach (var item in _items.OrderByDescending(x => x.Volume))
		{
			if (!this.TryFit(item))
				break;
		}

		return resultBuilder
			.WithFittedItems(_items.Where(x => x.Fitted))
			.WithUnfittedItems(_items.Where(x => !x.Fitted))
			.Build(parameters);
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
		return _availableSpace!.FirstOrDefault(x => x.Length >= orientation.Length && x.Width >= orientation.Width && x.Height >= orientation.Height);
	}

	private void Fit(VolumetricItem spaceQuadrant, VolumetricItem orientation, Item item)
	{
		var newAvailableSpaces = this.SplitSpaceQuadrant(spaceQuadrant, orientation);
		_availableSpace!.Remove(spaceQuadrant);
		if (newAvailableSpaces.Any())
		{
			_availableSpace.AddRange(newAvailableSpaces);
		}
		item.Fitted = true;
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
