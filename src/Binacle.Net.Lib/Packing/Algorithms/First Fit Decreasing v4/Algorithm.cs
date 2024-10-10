﻿using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Models;
using Binacle.Net.Lib.Exceptions;
using Binacle.Net.Lib.GuardClauses;
using System.Buffers;

namespace Binacle.Net.Lib.Packing.Algorithms;

internal partial class FirstFitDecreasing_v4<TBin, TItem> : IPackingAlgorithm
	where TBin : class, IWithID, IWithReadOnlyDimensions
	where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public string Name => "First Fit Decreasing";
	public int Version => 4;

	private readonly Bin bin;
	private readonly Item[] items;
	private readonly int totalItemsVolume;
	private readonly int totalItemCount;

	internal FirstFitDecreasing_v4(TBin bin, IList<TItem> items)
	{
		Guard.Against
			.Null(bin)
			.ZeroOrNegativeDimensions(bin)
			.NullOrEmpty(items);

		this.bin = new Bin(bin);

		// I think this is overkill since we check the dimensions
		Guard.Against
			.ZeroOrNegativeVolume(this.bin);

		this.totalItemCount = 0;
		for (int i = 0; i < items.Count; i++)
		{
			var item = items[i];

			Guard.Against
				.ZeroOrNegativeDimensions(item)
				.ZeroOrNegativeQuantity(item);

			this.totalItemCount += items[i].Quantity;
		}
		
		this.items = ArrayPool<Item>.Shared.Rent(this.totalItemCount);
		this.totalItemCount = 0;
		for (int i = 0; i < items.Count; i++)
		{
			var incomingItem = items[i];
			var incomingItemVolume = incomingItem.CalculateVolume();
			var incomingItemLongestDimension = incomingItem.CalculateLongestDimension();
			
			for (var quantity = 1; quantity <= incomingItem.Quantity; quantity++)
			{
				this.items[this.totalItemCount] = new Item(incomingItem, incomingItemVolume, incomingItemLongestDimension);
				this.totalItemsVolume += incomingItemVolume;

				this.totalItemCount++;
			}
		}

	}
}