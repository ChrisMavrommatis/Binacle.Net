using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.Strategies.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1 :
	IFittingAlgorithm,
	IFittingAlgorithmWithBins,
	IFittingAlgorithmWithBinsAndItems
{
	private List<VolumetricItem> _availableSpace;
	private List<Item> _fittedItems;
	private IEnumerable<Bin> _bins;
	private IEnumerable<Item> _items;

	internal FirstFitDecreasing_v1()
	{

	}

	public IFittingAlgorithmWithBins WithBins<TBin>(IEnumerable<TBin> bins)
		 where TBin : class, IWithID, IWithReadOnlyDimensions<int>
	{
		_bins = bins.Select(x => new Bin(x.ID, x)).ToList();
		return this;
	}

	public IFittingAlgorithmWithBinsAndItems AndItems<TItem>(IEnumerable<TItem> items)
		 where TItem : class, IWithID, IWithReadOnlyDimensions<int>
	{
		_items = items.Select(x => new Item(x.ID, x)).ToList();
		return this;
	}

	public IFittingAlgorithmOperation Build()
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
