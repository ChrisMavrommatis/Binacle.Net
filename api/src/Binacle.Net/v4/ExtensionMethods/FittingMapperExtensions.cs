using Binacle.Net.ExtensionMethods;
using Binacle.Net.v4.Contracts.Fit;

namespace Binacle.Net.v4.ExtensionMethods;

internal static class FittingMapperExtensions
{
	public static BinFitResultStatus MapToBinFitResultStatus(this OperationResultStatus operationResultStatus)
	{
		return operationResultStatus switch
		{
			OperationResultStatus.Unknown => BinFitResultStatus.Unknown,
			OperationResultStatus.FullyPacked => BinFitResultStatus.Fits,
			OperationResultStatus.PartiallyPacked => BinFitResultStatus.DoesNotFit,
			OperationResultStatus.NotPacked => BinFitResultStatus.DoesNotFit,
			OperationResultStatus.EarlyExit => BinFitResultStatus.EarlyExit,
			_ => throw new NotSupportedException(
				$"No Bin Fit Result Status Implementation exists for operation result status {operationResultStatus.ToFastString()}"
			)
		};
	}

	public static BinFitEarlyExitReason MapToBinFitEarlyExitReason(this EarlyExitReason earlyExitReason)
	{
		return earlyExitReason switch
		{
			EarlyExitReason.None => BinFitEarlyExitReason.None,
			EarlyExitReason.ContainerVolumeExceeded => BinFitEarlyExitReason.ContainerVolumeExceeded,
			EarlyExitReason.ContainerDimensionExceeded => BinFitEarlyExitReason.ContainerDimensionExceeded,
			_ => throw new NotSupportedException(
				$"No Bin Fit Early Exit Reason Implementation exists for Early Exit Reason {earlyExitReason.ToFastString()}"
			)
		};
	}
}
