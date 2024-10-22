using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.GuardClauses;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v3<TBin, TItem> : IFittingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string Name => "First Fit Decreasing";
	public int Version => 3;

	private List<VolumetricItem>? availableSpace;
	private readonly Bin bin;

	// List -> Array  v2-v3
	// 10  items 3.39 -> 3.29   KB
	// 70  items 15.57 -> 15.47 KB
	// 130 items 27.2 -> 27.1 KB
	// 192 items 38.02 -> 37.92 KB
	// 202 Items 13.01 -> 12.95 KB
	private readonly Item[] items;
	private int totalItemsVolume;

	internal FirstFitDecreasing_v3(TBin bin, IList<TItem> items)
	{
		Guard.Against
			.Null(bin)
			.ZeroOrNegativeDimensions(bin)
			.NullOrEmpty(items);

		this.bin = new Bin(bin);

		var totalItemCount = 0;
		for (int i = 0; i < items.Count; i++)
		{
			var item = items[i];

			Guard.Against
				.ZeroOrNegativeDimensions(item)
				.ZeroOrNegativeQuantity(item);

			totalItemCount += items[i].Quantity;
		}

		this.items = new Item[totalItemCount];
		totalItemCount = 0;
		for (int i = 0; i < items.Count; i++)
		{
			var incomingItem = items[i];
			var incomingItemVolume = incomingItem.CalculateVolume();
			var incomingItemLongestDimension = incomingItem.CalculateLongestDimension();

			for (var quantity = 1; quantity <= incomingItem.Quantity; quantity++)
			{
				this.items[totalItemCount] = new Item(incomingItem.ID, incomingItem);
				this.totalItemsVolume += incomingItemVolume;
				totalItemCount++;
			}
		}
	}
}
