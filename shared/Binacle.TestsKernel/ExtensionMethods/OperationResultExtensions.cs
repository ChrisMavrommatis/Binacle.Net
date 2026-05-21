using Binacle.Lib.Abstractions.Models;

namespace Binacle.TestsKernel.ExtensionMethods;

public static class OperationResultExtensions
{
	public static int TotalPackedItemsVolume(this OperationResult result)
	{
		return result.PackedItems.Sum(x => x.Volume);
	}
	
	public static int TotalUnpackedItemsVolume(this OperationResult result)
	{
		return result.UnpackedItems.Sum(x => x.Volume * x.Quantity);
	}
	
	public static int TotalItemsVolume(this OperationResult result)
	{
		return result.TotalPackedItemsVolume() + result.TotalUnpackedItemsVolume();
	}

	public static int TotalItemsCount(this OperationResult result)
	{
		return result.PackedItems.Count + result.UnpackedItems.Sum(x => x.Quantity);
	}
}
