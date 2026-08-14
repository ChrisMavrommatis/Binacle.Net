namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591
internal static class AlgorithmExtensions
{
	internal static Binacle.Packing.Algorithm ToLibAlgorithm(this Algorithm algorithm)
	{
		return algorithm switch
		{
			Algorithm.FFD => Binacle.Packing.Algorithm.FFD,
			Algorithm.WFD => Binacle.Packing.Algorithm.WFD,
			Algorithm.BFD => Binacle.Packing.Algorithm.BFD,
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
		};
	}
}
