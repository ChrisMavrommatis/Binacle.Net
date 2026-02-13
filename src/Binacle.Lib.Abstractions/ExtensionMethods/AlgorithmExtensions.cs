using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib;

public static class AlgorithmExtensions
{
	public static string GetAlgorithmIdentifierName(this IPackingAlgorithm algorithm)
	{
		return $"{algorithm.GetAlgorithmShortName()}_v{algorithm.Version}";
	}
	
	public static string GetAlgorithmShortName(this IPackingAlgorithm algorithm)
	{
		return algorithm.Algorithm switch
		{
			Algorithm.FFD => $"FFD",
			Algorithm.BFD => $"BFD",
			Algorithm.WFD => $"WFD",
			_ => $"UNK"
		};
	}
}
