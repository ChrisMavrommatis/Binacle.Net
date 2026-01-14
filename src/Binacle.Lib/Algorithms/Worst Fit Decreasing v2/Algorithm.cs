using Binacle.Lib.GuardClauses;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Algorithms;

internal partial class WorstFitDecreasing_v2<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public Algorithm Algorithm => Algorithm.WorstFitDecreasing;
	public int Version => 2;

	private readonly Bin bin;
	private readonly Item[] items;
	private readonly int totalItemsVolume;

	internal WorstFitDecreasing_v2(TBin bin, IList<TItem> items)
	{
		Guard.Against
			.Null(bin)
			.ZeroOrNegativeDimensions(bin)
			.NullOrEmpty(items);

		this.bin = new Bin(bin);

		// I think this is overkill since we check the dimensions
		Guard.Against
			.ZeroOrNegativeVolume(this.bin);

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
				this.items[totalItemCount] = new Item(incomingItem, incomingItemVolume, incomingItemLongestDimension);
				this.totalItemsVolume += incomingItemVolume;

				totalItemCount++;
			}
		}

	}
}
