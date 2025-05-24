using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class Packing_BFD : SingleScenarioScalingBase
{
	public Packing_BFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(0)]
	public PackingResult Packing_BFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_BFD_v1(this.Bin!, this.Items!);
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
	public PackingResult Packing_BFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_BFD_v2(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			OptInToEarlyFails = false, 
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
}
