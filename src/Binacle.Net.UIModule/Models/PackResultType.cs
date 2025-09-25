namespace Binacle.Net.UIModule.Models;

internal enum PackResultType
{
	Unknown,
	NotPacked,
	PartiallyPacked,
	FullyPacked,
	EarlyFail_ContainerVolumeExceeded,
	EarlyFail_ContainerDimensionExceeded,
}
