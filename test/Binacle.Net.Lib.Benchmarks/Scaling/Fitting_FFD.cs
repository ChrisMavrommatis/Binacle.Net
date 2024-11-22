using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Binacle.Net.Lib.Fitting.Models;

namespace Binacle.Net.Lib.Benchmarks.Scaling;

// [SimpleJob(RuntimeMoniker.Net80, baseline: true)]
// [SimpleJob(RuntimeMoniker.Net90)]
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
