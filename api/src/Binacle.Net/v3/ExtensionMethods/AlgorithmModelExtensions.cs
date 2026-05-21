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
    
    internal static Binacle.Lib.Algorithm ToLibAlgorithm(this Contracts.Algorithm? algorithm)
	{
		return algorithm switch
		{
			Contracts.Algorithm.FFD => Binacle.Lib.Algorithm.FFD,
			Contracts.Algorithm.WFD => Binacle.Lib.Algorithm.WFD,
			Contracts.Algorithm.BFD => Binacle.Lib.Algorithm.BFD,
			_ => throw new NotSupportedException($"Algorithm {algorithm} is not supported.")
		};
	}
}
