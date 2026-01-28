using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib.Models;

internal class OperationResultBuilder<TBin, TItem>
	where TBin : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume
	where TItem : IWithID, IWithReadOnlyDimensions, IWithReadOnlyVolume, IWithReadOnlyCoordinates
{
	private readonly AlgorithmInfo algorithmInfo;
	private readonly TBin bin;
	private readonly int totalItems;
	private readonly int totalItemsVolume;
	private readonly IOperationParameters operationParameters;
	private IEnumerable<TItem>? packedItems;
	private IEnumerable<IGrouping<string, TItem>>? unpackedItems;
	
	internal OperationResultBuilder(
		AlgorithmInfo algorithmInfo, 
		TBin bin,
		int totalItems, 
		int totalItemsVolume,
		IOperationParameters operationParameters)
	{
		this.algorithmInfo = algorithmInfo;
		this.bin = bin;
		this.totalItems = totalItems;
		this.totalItemsVolume = totalItemsVolume;
		this.operationParameters = operationParameters;
	}
	
	internal OperationResultBuilder<TBin, TItem> WithPackedItems(IEnumerable<TItem> items)
	{
		this.packedItems = items;
		return this;
	}
	internal OperationResultBuilder<TBin, TItem> WithUnpackedItems(IEnumerable<TItem> items)
	{
		this.unpackedItems = items.GroupBy(x => x.ID);
		return this;
	}

	internal OperationResult EarlyExit(EarlyExitReason reason)
	{
		var result = this.Complete();
		result.Status = reason switch
		{
			EarlyExitReason.ContainerDimensionExceeded => OperationResultStatus.EarlyFail_ContainerDimensionExceeded,
			EarlyExitReason.ContainerVolumeExceeded => OperationResultStatus.EarlyFail_ContainerVolumeExceeded,
			_ => throw new NotImplementedException($"The specified early exit reason {reason} is not implemented.")
		};
		return result;
	}
	
	internal OperationResult Complete()
	{
		using var activity = Diagnostics.ActivitySource
			.StartActivity("Build Operation Result");
		
		var packedItemsListSize = this.packedItems?.Count() ?? 0;
		var unpackedItemsListSize = this.unpackedItems?.Count() ?? 0;
		
		var packedResultItems = new List<PackedItem>(packedItemsListSize);
		var unpackedResultItems = new List<UnpackedItem>(unpackedItemsListSize);
		
		var packedItemsCount = 0;
		var packedItemsVolume = 0;
		
		var unpackedItemsCount = 0;
		var unpackedItemsVolume = 0;
		
		if(this.packedItems is not null)
		{
			foreach(var packedItem in this.packedItems)
			{
				packedItemsCount++;
				packedItemsVolume += packedItem.Volume;

				packedResultItems.Add(
					new PackedItem(packedItem.ID, packedItem, packedItem)
				);
			}
		}
		
		if(this.unpackedItems is not null)
		{
			foreach (var unpackedItemGroup in this.unpackedItems)
			{
				var first = unpackedItemGroup.First();
				var quantity = unpackedItemGroup.Count();
				unpackedItemsCount += quantity;
				unpackedItemsVolume += (first.Volume * quantity);

				unpackedResultItems.Add(
					new UnpackedItem(unpackedItemGroup.Key, first, quantity)
				);
			}
		}
		
		var totalItemsCount = packedItemsCount + unpackedItemsCount;
		if (totalItemsCount != this.totalItems)
		{
			throw new InvalidOperationException($"The expected total items count is {this.totalItems} but was {totalItemsCount}");
		}
		
		var calculatedTotalItemsVolume = packedItemsVolume + unpackedItemsVolume;

		if (calculatedTotalItemsVolume != this.totalItemsVolume)
		{
			throw new InvalidOperationException($"The expected total items volume is {this.totalItemsVolume} but was {calculatedTotalItemsVolume}");
		}

		var packedBinVolumePercentage = Math.Round((decimal)packedItemsVolume / this.bin.Volume * 100, 2);

		var packedItemsVolumePercentage = Math.Round((decimal)packedItemsVolume / calculatedTotalItemsVolume * 100, 2);

		var result = new OperationResult()
		{
			Bin = new PackedBin(this.bin.ID, this.bin),
			Status = OperationResultStatus.Unknown,
			AlgorithmInfo = this.algorithmInfo,
			PackedItems = packedResultItems.AsReadOnly(),
			UnpackedItems = unpackedResultItems.AsReadOnly(),
			PackedBinVolumePercentage = packedBinVolumePercentage,
			PackedItemsVolumePercentage = packedItemsVolumePercentage
		};
		if (packedItemsCount == this.totalItems)
		{
			result.Status = OperationResultStatus.FullyPacked;
		}
		else if (unpackedItemsCount == this.totalItems)
		{
			result.Status = OperationResultStatus.NotPacked;
		}
		else if (packedItemsCount > 0)
		{
			result.Status = OperationResultStatus.PartiallyPacked;
		}

		return result;
	}
}
