using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Binacle.Net.Lib.Benchmarks.Order;
using Binacle.Net.Lib.Packing.Models;

namespace Binacle.Net.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class Packing_FFD : SingleScenarioScalingBase
{
	public Packing_FFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(0)]
	public PackingResult Packing_FFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_FFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}

	[Benchmark]
	[BenchmarkOrder(1)]
	public PackingResult Packing_FFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_FFD_v2(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			OptInToEarlyFails = false, 
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
}
