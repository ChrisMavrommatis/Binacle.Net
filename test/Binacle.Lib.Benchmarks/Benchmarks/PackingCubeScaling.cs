using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class PackingCubeScaling : CubeScalingBenchmarkBase
{
	public PackingCubeScaling() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public PackingResult FFD_v1()
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
	[BenchmarkOrder(11)]
	public PackingResult FFD_v2()
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

	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult WFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_WFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(21)]
	public PackingResult WFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_WFD_v2(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}

	[Benchmark]
	[BenchmarkOrder(30)]
	public PackingResult BFD_v1()
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
	[BenchmarkOrder(31)]
	public PackingResult BFD_v2()
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
