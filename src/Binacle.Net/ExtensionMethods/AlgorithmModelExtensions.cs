using Binacle.Lib;

namespace Binacle.Net.ExtensionMethods;

internal static class AlgorithmModelExtensions
{
	internal static Binacle.Lib.Algorithm ToLibAlgorithm(this Models.Algorithm algorithm)
	{
		return algorithm switch
		{
			Models.Algorithm.FFD => Binacle.Lib.Algorithm.FirstFitDecreasing,
			Models.Algorithm.WFD => Binacle.Lib.Algorithm.WorstFitDecreasing,
			Models.Algorithm.BFD => Binacle.Lib.Algorithm.BestFitDecreasing,
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
		};
	}
	
	internal static string ToFastString(this Models.Algorithm algorithm)
	{
		return algorithm switch
		{
			Models.Algorithm.FFD => nameof(Models.Algorithm.FFD),
			Models.Algorithm.WFD => nameof(Models.Algorithm.WFD),
			Models.Algorithm.BFD => nameof(Models.Algorithm.BFD),
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
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
