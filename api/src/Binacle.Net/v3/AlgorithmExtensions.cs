namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591
internal static class AlgorithmExtensions
{
	internal static Binacle.Lib.Algorithm ToLibAlgorithm(this Algorithm algorithm)
	{
		return algorithm switch
		{
			Algorithm.FFD => Binacle.Lib.Algorithm.FFD,
			Algorithm.WFD => Binacle.Lib.Algorithm.WFD,
			Algorithm.BFD => Binacle.Lib.Algorithm.BFD,
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
		};
	}
}
