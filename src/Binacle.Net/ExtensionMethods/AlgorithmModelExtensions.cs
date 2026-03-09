using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Net.ExtensionMethods;

internal static class AlgorithmModelExtensions
{
	internal static string ToFastString(this Algorithm algorithm)
	{
		return algorithm switch
		{
			Algorithm.FFD => nameof(Algorithm.FFD),
			Algorithm.WFD => nameof(Algorithm.WFD),
			Algorithm.BFD => nameof(Algorithm.BFD),
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
		};
	}
	
	internal static string ToFastString(this OperationResultStatus operationResultStatus)
	{
		return operationResultStatus switch
		{ 
			OperationResultStatus.Unknown => nameof(OperationResultStatus.Unknown),
			OperationResultStatus.FullyPacked => nameof(OperationResultStatus.FullyPacked),
			OperationResultStatus.PartiallyPacked => nameof(OperationResultStatus.PartiallyPacked),
			OperationResultStatus.NotPacked => nameof(OperationResultStatus.NotPacked),
			OperationResultStatus.EarlyFail_ContainerDimensionExceeded => nameof(OperationResultStatus.EarlyFail_ContainerDimensionExceeded),
			OperationResultStatus.EarlyFail_ContainerVolumeExceeded => nameof(OperationResultStatus.EarlyFail_ContainerVolumeExceeded),
			_ => throw new NotSupportedException($"Operation result status {operationResultStatus} is not supported.")
		};
	}
	
	internal static string ToFastString(this AlgorithmOperation operation)
	{
		return operation switch
		{
			AlgorithmOperation.Fitting => nameof(AlgorithmOperation.Fitting),
			AlgorithmOperation.Packing => nameof(AlgorithmOperation.Packing),
			_ => throw new NotSupportedException($"Algorithm operation {operation} is not supported.")
		};
	}
}
