using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.GuardClauses;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v2<TBin, TItem> : IFittingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string Name => "First Fit Decreasing";
	public int Version => 2;

	private List<VolumetricItem> availableSpace;
	private Bin bin;
	private List<Item> items;

	internal FirstFitDecreasing_v2(TBin bin, IEnumerable<TItem> items)
	{
		Guard.Against
			.Null(bin)
			.ZeroOrNegativeDimensions(bin)
			.NullOrEmpty(items);

		this.bin = new Bin(bin.ID, bin);
		this.items = new List<Item>();
		foreach (var incomingItem in items)
		{
			Guard.Against
			.ZeroOrNegativeDimensions(incomingItem)
			.ZeroOrNegativeQuantity(incomingItem);

			for(var i = 0; i< incomingItem.Quantity; i++)
			{
				this.items.Add(new Item(incomingItem.ID, incomingItem));
			}
		}
	}
}
