namespace Binacle.Net.v4.Contracts;

public enum OperationResultStatus
{
	Unknown = -1,
	FullyPacked = 0,                          // FullyPacked
	PartiallyPacked = 1,                      // PartiallyPacked (includes at least 1 items packed)
	NotPacked = 2,                            // NotPacked
	EarlyFail_ContainerVolumeExceeded = 10,
	EarlyFail_ContainerDimensionExceeded = 11
}


