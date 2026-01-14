namespace Binacle.Lib.Abstractions.Models;

public readonly struct AlgorithmInfo
{
	public readonly AlgorithmOperation Operation;
	public readonly Algorithm Algorithm;
	public readonly int Version;

	public AlgorithmInfo(Algorithm algorithm, int version, AlgorithmOperation operation)
	{
		this.Operation = operation;
		this.Algorithm = algorithm;
		this.Version = version;
	}
	
}
