using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.GuardClauses;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1<TBin, TItem> : IFittingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string Name => "First Fit Decreasing";
	public int Version => 1;

	private List<VolumetricItem> _availableSpace;
	private Bin _bin;
	private List<Item> _items;

	internal FirstFitDecreasing_v1(TBin bin, IEnumerable<TItem> items)
	{
		Guard.Against
			.Null(bin)
			.ZeroOrNegativeDimensions(bin)
			.NullOrEmpty(items);

		_bin = new Bin(bin.ID, bin);
		_items = items.SelectMany((incomingItem, index) =>
		{
			Guard.Against
			.ZeroOrNegativeDimensions(incomingItem)
			.ZeroOrNegativeQuantity(incomingItem);

			// create new items as many as quantity
			return Enumerable.Range(0, incomingItem.Quantity)
				.Select(i => new Item(incomingItem.ID, incomingItem));
		}).ToList();
	}
}
