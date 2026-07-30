using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;

namespace Binacle.Lib;

public static class AlgorithmExtensions
{
	public static string GetAlgorithmIdentifierName(this IPackingAlgorithm algorithm)
	{
		return $"{algorithm.Algorithm.ToShortName()}_v{algorithm.Version}";
	}
	
	public static string GetAlgorithmIdentifierName(this AlgorithmInfo algorithmInfo)
	{
		return $"{algorithmInfo.Algorithm.ToShortName()}_v{algorithmInfo.Version}";
	}
	
	private static string ToShortName(this Algorithm algorithm)
	{
		return algorithm switch
		{
			Algorithm.FFD => $"FFD",
			Algorithm.BFD => $"BFD",
			Algorithm.WFD => $"WFD",
			_ => $"UNK"
		};
	}
}
