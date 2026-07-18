using System.ComponentModel;
namespace Binacle.Net.v4.Contracts.Fit;

#pragma warning disable CS1591

[Description("Why a fit check stopped early, or None if it ran to completion.")]
public enum BinFitEarlyExitReason
{
	None = 0,
	ContainerVolumeExceeded = 1,
	ContainerDimensionExceeded = 2,
}
