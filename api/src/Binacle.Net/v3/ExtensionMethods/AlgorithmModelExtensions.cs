namespace Binacle.Net.v3.ExtensionMethods;

internal static class AlgorithmModelExtensions
{
    internal static string ToFastString(this Contracts.Algorithm? algorithm)
    {
        return algorithm switch
        {
            Contracts.Algorithm.FFD => nameof(Contracts.Algorithm.FFD),
            Contracts.Algorithm.WFD => nameof(Contracts.Algorithm.WFD),
            Contracts.Algorithm.BFD => nameof(Contracts.Algorithm.BFD),
            _ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
        };
    }
    
    internal static Binacle.Packing.Algorithm ToLibAlgorithm(this Contracts.Algorithm? algorithm)
	{
		return algorithm switch
		{
			Contracts.Algorithm.FFD => Binacle.Packing.Algorithm.FFD,
			Contracts.Algorithm.WFD => Binacle.Packing.Algorithm.WFD,
			Contracts.Algorithm.BFD => Binacle.Packing.Algorithm.BFD,
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
		};
	}
}
