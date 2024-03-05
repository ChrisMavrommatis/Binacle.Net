using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Abstractions.Strategies;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.Models;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Strategies;

internal sealed partial class FirstFitDecreasing_v1 :
	IBinFittingStrategy,
	IBinFittingStrategyWithBins,
	IBinFittingStrategyWithBinsAndItems
{
	private List<VolumetricItem> _availableSpace;
	private List<Item> _fittedItems;
	private IEnumerable<Bin> _bins;
	private IEnumerable<Item> _items;

	internal FirstFitDecreasing_v1()
	{

	}

	public IBinFittingStrategyWithBins WithBins<TBin>(IEnumerable<TBin> bins)
		 where TBin : class, IItemWithReadOnlyDimensions<int>
	{
		_bins = bins.Select(x => new Bin(x.ID, x)).ToList();
		return this;
	}

	public IBinFittingStrategyWithBinsAndItems AndItems<TItem>(IEnumerable<TItem> items)
		 where TItem : class, IItemWithReadOnlyDimensions<int>
	{
		_items = items.Select(x => new Item(x.ID, x)).ToList();
		return this;
	}

	public IBinFittingOperation Build()
	{
		if (!(_bins?.Any() ?? false))
			throw new ArgumentNullException($"{nameof(_bins)} is empty. At least one bin is required");

		if (!(_items?.Any() ?? false))
			throw new ArgumentNullException($"{nameof(_items)} is empty. At least one item is required");

		if (_bins.Any(x => x.Volume <= 0))
			throw new DimensionException("Volume", "You cannot have a bin with negative or 0 volume");

		if (_items.Any(x => x.Volume <= 0))
			throw new DimensionException("Volume", "You cannot have an item with negative or 0 volume");

		return this;
	}
}
