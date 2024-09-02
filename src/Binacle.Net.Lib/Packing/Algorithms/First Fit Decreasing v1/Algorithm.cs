using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;

namespace Binacle.Net.Lib.Packing.Algorithms;

internal partial class FirstFitDecreasing_v1<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string AlgorithmName => "First Fit Decreasing";
	public int Version => 3;

	private readonly Bin bin;
	private readonly List<Item> items;
	private readonly int totalItemsVolume;

	internal FirstFitDecreasing_v1(TBin bin, IList<TItem> items)
	{
		if(bin is null)
		{
			throw new ArgumentNullException($"{nameof(bin)} is null. A bin is required");
		}

		if(!(items?.Any() ?? false))
		{
			throw new ArgumentNullException($"{nameof(items)} is empty. At least one item is required");
		}

		this.bin = new Bin(bin);

		if (this.bin.Volume <= 0)
		{
			throw new DimensionException("Volume", "You cannot have a bin with negative or 0 volume");
		}

		var totalItemCount = 0;
		for (int i = 0; i < items.Count; i++)
		{
			totalItemCount += items[i].Quantity;
		}
		if (totalItemCount <= 0)
		{
			throw new ArgumentException("You need to provide items with 1 or more quantity", nameof(items));
		}
		this.items = new List<Item>(totalItemCount);
		
		for (int i = 0; i < items.Count; i++)
		{
			var incomingItem = items[i];
			var incomingItemVolume = incomingItem.CalculateVolume();
			var incomingItemLongestDimension = incomingItem.CalculateLongestDimension();
			if (incomingItem.Quantity <= 0)
			{
				throw new DimensionException("Quantity", "You cannot have an item with negative or 0 quantity");
			}
			if (incomingItemVolume <= 0)
			{
				throw new DimensionException("Volume", "You cannot have an item with negative or 0 volume");
			}

			for (var quantity = 1; quantity <= incomingItem.Quantity; quantity++)
			{
				this.items.Add(new Item(incomingItem, quantity, incomingItemVolume, incomingItemLongestDimension));
				this.totalItemsVolume += incomingItemVolume;
			}
		}

	}
}
