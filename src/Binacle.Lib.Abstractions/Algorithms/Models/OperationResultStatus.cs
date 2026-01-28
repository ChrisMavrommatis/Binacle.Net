namespace Binacle.Lib.Abstractions.Models;

public enum OperationResultStatus
{
	Unknown = -1,
	FullyPacked = 0,                            // FullyPacked
	PartiallyPacked = 1,                            // PartiallyPacked (includes at least 1 items packed)
	NotPacked = 2,                            // NotPacked
	EarlyFail_ContainerVolumeExceeded = 10,
	EarlyFail_ContainerDimensionExceeded = 11
}

public sealed class OperationResult
{
	internal OperationResult()
	{
		
	}

	public required PackedBin Bin { get; init; }
	
	public required AlgorithmInfo AlgorithmInfo { get; init; }
	public OperationResultStatus Status { get; internal set; }

	public required IReadOnlyList<PackedItem> PackedItems { get; init; }
	public required IReadOnlyList<UnpackedItem> UnpackedItems { get; init; }

	public required decimal PackedItemsVolumePercentage { get; init; }
	public required decimal PackedBinVolumePercentage { get; init; }
}
