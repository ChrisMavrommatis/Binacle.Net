using Binacle.Net.Kernel.Logs.Models;

namespace Binacle.Net.Models;

internal class PackingParameters : ILogConvertible
{
	public required Algorithm Algorithm { get; init; }

	internal Binacle.Lib.Algorithm GetMappedAlgorithm()
	{
		return this.Algorithm switch
		{
			Algorithm.FFD => Binacle.Lib.Algorithm.FirstFitDecreasing,
			Algorithm.WFD => Binacle.Lib.Algorithm.WorstFitDecreasing,
			Algorithm.BFD => Binacle.Lib.Algorithm.BestFitDecreasing,
			_ => throw new NotSupportedException($"Algorithm {this.Algorithm} is not supported.")
		};
		
	}

	public object ConvertToLogObject()
	{
		List<string> parameters =
		[
			this.Algorithm.ToString()
		];
		return parameters;
	}
}
