using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.GuardClauses;

namespace Binacle.Net.Lib.Packing.Algorithms;

[Obsolete("This class is obsolete. Use FirstFitDecreasing_v2 instead.")]
internal partial class FirstFitDecreasing_v1<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string Name => "First Fit Decreasing";
	public int Version => 1;

	private readonly Bin bin;
	private readonly List<Item> items;
	private readonly int totalItemsVolume;

	internal FirstFitDecreasing_v1(TBin bin, IList<TItem> items)
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
		
		this.items = new List<Item>(totalItemCount);
		
		for (int i = 0; i < items.Count; i++)
		{
			var incomingItem = items[i];
			var incomingItemVolume = incomingItem.CalculateVolume();
			
			for (var quantity = 1; quantity <= incomingItem.Quantity; quantity++)
			{
				this.items.Add(new Item(incomingItem));
				this.totalItemsVolume += incomingItemVolume;
			}
		}

	}
}
