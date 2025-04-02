using Binacle.Net.Api.Kernel.Logs.Models;

namespace Binacle.Net.Api.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PackingParameters : ILogConvertible
{
	public required Algorithm Algorithm { get; init; }

	internal Lib.Algorithm GetMappedAlgorithm()
	{
		return this.Algorithm switch
		{
			Algorithm.FFD => Lib.Algorithm.FirstFitDecreasing,
			Algorithm.WFD => Lib.Algorithm.WorstFitDecreasing,
			Algorithm.BFD => Lib.Algorithm.BestFitDecreasing,
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
