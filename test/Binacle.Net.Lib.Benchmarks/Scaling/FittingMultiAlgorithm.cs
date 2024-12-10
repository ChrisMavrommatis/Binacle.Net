using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class FittingMultiAlgorithm : SingleScenarioScalingBase
{
	public FittingMultiAlgorithm() : base("Rectangular-Cuboids::Small")
	{
	}

	private readonly AlgorithmFactory<IFittingAlgorithm>[] algorithmFactories =
	[
		AlgorithmFactories.Fitting_FFD_v1,
		AlgorithmFactories.Fitting_WFD_v1,
		AlgorithmFactories.Fitting_BFD_v1,
	];

	[Benchmark(Baseline = true)]
	public FittingResult ForLoop()
	{
		var results = new FittingResult[this.algorithmFactories.Length];
		for (var i = 0; i < this.algorithmFactories.Length; i++)
		{
			var algorithmFactory = this.algorithmFactories[i];
			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
			var result = algorithmInstance.Execute(new FittingParameters
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[i] = result;
		}

		return results
			.OrderByDescending(x => x.FittedBinVolumePercentage)
			.FirstOrDefault()!;
	}

	[Benchmark]
	public FittingResult ParallelFor()
	{
		var results = new FittingResult[this.algorithmFactories.Length];
		Parallel.For(0, this.algorithmFactories.Length, index =>
		{
			var algorithmFactory = this.algorithmFactories[index];
			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
			var result = algorithmInstance.Execute(new FittingParameters
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[index] = result;
		});
		return results
			.OrderByDescending(x => x.FittedBinVolumePercentage)
			.FirstOrDefault()!;
	}
}
