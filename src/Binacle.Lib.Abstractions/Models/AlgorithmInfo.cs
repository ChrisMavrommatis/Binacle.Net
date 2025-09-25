namespace Binacle.Lib.Abstractions.Models;

public readonly struct AlgorithmInfo
{
	public Algorithm Algorithm { get; }
	public string Name { get; }
	public int Version  { get; }

	public AlgorithmInfo(Algorithm algorithm, string name, int version)
	{
		this.Algorithm = algorithm;
		this.Name = name;
		this.Version = version;
	}
	
}
