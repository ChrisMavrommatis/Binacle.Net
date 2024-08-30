namespace Binacle.Net.Lib;

public enum BinPackingResultStatus
{
	Unknown,
	FullyPacked,
	PartiallyPacked,
	NotPacked,
	EarlyFail_ContainerVolumeExceeded,
	EarlyFail_ContainerDimensionExceeded,
	Error
}
