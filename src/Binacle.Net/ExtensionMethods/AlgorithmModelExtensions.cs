using Binacle.Lib;

namespace Binacle.Net.ExtensionMethods;

internal static class AlgorithmModelExtensions
{
	internal static string ToFastString(this Lib.Algorithm algorithm)
	{
		return algorithm switch
		{
			Binacle.Lib.Algorithm.FFD => nameof(Binacle.Lib.Algorithm.FFD),
			Binacle.Lib.Algorithm.WFD => nameof(Binacle.Lib.Algorithm.WFD),
			Binacle.Lib.Algorithm.BFD => nameof(Binacle.Lib.Algorithm.BFD),
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
