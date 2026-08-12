namespace Binacle.Packing;

public readonly struct AlgorithmInfo
{
	public readonly Algorithm Algorithm;
	public readonly int Version;

	public AlgorithmInfo(Algorithm algorithm, int version)
	{
		this.Algorithm = algorithm;
		this.Version = version;
	}
}
