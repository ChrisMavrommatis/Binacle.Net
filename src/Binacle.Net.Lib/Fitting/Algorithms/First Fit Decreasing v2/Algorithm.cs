﻿using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v2 :
	IFittingAlgorithm,
	IFittingAlgorithmWithBins,
	IFittingAlgorithmWithBinsAndItems
{
	private List<VolumetricItem> availableSpace;
	private List<Item> fittedItems;
	private IEnumerable<Bin> bins;
	private IEnumerable<Item> items;

	internal FirstFitDecreasing_v2()
	{
	}

	public IFittingAlgorithmWithBins WithBins<TBin>(IEnumerable<TBin> bins)
		where TBin : class, IWithID, IWithReadOnlyDimensions
	{
		var _bins = new List<Bin>();

		if (bins is not null)
		{
			foreach (var incomingBin in bins)
			{
				_bins.Add(new Bin(incomingBin.ID, incomingBin));
			}
		}

		this.bins = _bins;
		return this;
	}

	public IFittingAlgorithmWithBinsAndItems AndItems<TItem>(IEnumerable<TItem> items)
		where TItem : class, IWithID, IWithReadOnlyDimensions

	{
		var _items = new List<Item>();

		if (items is not null)
		{
			foreach (var incomingItem in items)
			{
				_items.Add(new Item(incomingItem.ID, incomingItem));
			}
		}

		this.items = _items;
		return this;
	}

	public IFittingAlgorithmOperation Build()
	{
		if (!(this.bins?.Any() ?? false))
			throw new ArgumentNullException($"{nameof(bins)} is empty. At least one bin is required");

		if (!(this.items?.Any() ?? false))
			throw new ArgumentNullException($"{nameof(items)} is empty. At least one item is required");

		if (this.bins.Any(x => x.Volume <= 0))
			throw new DimensionException("Volume", "You cannot have a bin with negative or 0 volume");

		if (this.items.Any(x => x.Volume <= 0))
			throw new DimensionException("Volume", "You cannot have an item with negative or 0 volume");

		return this;
	}
}
