using Binacle.Lib.GuardClauses;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Fitting.Algorithms;

internal sealed partial class BestFitDecreasing_v2<TBin, TItem> : IFittingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string Name => "Best Fit Decreasing";
	public int Version => 2;

	private List<VolumetricItem>? availableSpace;
	private Bin bin;
	private List<Item> items;
	private int totalItemsVolume;

	internal BestFitDecreasing_v2(TBin bin, IList<TItem> items)
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

		this.items = new List<Item>(totalItemCount);

		for (int i = 0; i < items.Count; i++)
		{
			var incomingItem = items[i];
			var incomingItemVolume = incomingItem.CalculateVolume();
			var incomingItemLongestDimension = incomingItem.CalculateLongestDimension();

			for (var quantity = 1; quantity <= incomingItem.Quantity; quantity++)
			{
				this.items.Add(new Item(incomingItem.ID, incomingItem));
				this.totalItemsVolume += incomingItemVolume;
			}
		}
	}
}
