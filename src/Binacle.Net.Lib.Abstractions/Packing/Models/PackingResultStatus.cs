namespace Binacle.Net.Lib.Packing.Models;

public enum PackingResultStatus
{
	Unknown,
	FullyPacked,
	PartiallyPacked,
	NotPacked,
	EarlyFail_ContainerVolumeExceeded,
	EarlyFail_ContainerDimensionExceeded,
	Error
}
