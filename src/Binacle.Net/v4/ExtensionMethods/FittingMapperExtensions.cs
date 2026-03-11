using Binacle.Lib.Abstractions.Models;
using Binacle.Net.v4.Contracts.Fit;

namespace Binacle.Net.v4.ExtensionMethods;

internal static class FittingMapperExtensions
{
	public static BinFitResultStatus MapToBinFitResultStatus(this OperationResultStatus operationResultStatus)
	{
		return operationResultStatus switch
		{
		};
	}

	public static BinFitEarlyExitReason MapToBinFitEarlyExitReason(this EarlyExitReason earlyExitReason)
	{
		return earlyExitReason switch
		{
			EarlyExitReason.None => BinFitEarlyExitReason.None,
			EarlyExitReason.ContainerVolumeExceeded => BinFitEarlyExitReason.ContainerVolumeExceeded,
			EarlyExitReason.ContainerDimensionExceeded => BinFitEarlyExitReason.ContainerDimensionExceeded
		};
	}
}
