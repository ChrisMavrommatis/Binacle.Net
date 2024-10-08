namespace Binacle.Net.Lib.Packing.Models;

public enum PackingResultStatus
{
	Unknown,
	NotPacked,
	PartiallyPacked,
	FullyPacked,
	EarlyFail_ContainerVolumeExceeded,
	EarlyFail_ContainerDimensionExceeded,
}
