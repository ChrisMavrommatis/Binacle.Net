using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class Fitting_FFD : SingleScenarioScalingBase
{
	public Fitting_FFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	public FittingResult Fitting_FFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false});
		return result;
	}

	[Benchmark]
	public FittingResult Fitting_FFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v2(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false });
		return result;
	}

	[Benchmark]
	public FittingResult Fitting_FFD_v3()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v3(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false });
		return result;
	}
}
