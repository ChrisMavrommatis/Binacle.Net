using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class FittingComparison : SingleScenarioScalingBase
{
	public FittingComparison() : base("Rectangular-Cuboids::Small")
	{
	}
	
	[Benchmark(Baseline = true)]
	public FittingResult Fitting_FFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false});
		return result;
	}
}
