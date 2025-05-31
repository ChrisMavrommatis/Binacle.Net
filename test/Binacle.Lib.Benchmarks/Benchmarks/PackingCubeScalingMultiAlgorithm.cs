using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class PackingCubeScalingMultiAlgorithm : CubeScalingBenchmarkBase
{
	public PackingCubeScalingMultiAlgorithm() : base("Rectangular-Cuboids::Small")
	{
	}

	private readonly AlgorithmFactory<IPackingAlgorithm>[] algorithmFactories_v1 =
	[
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_WFD_v1,
		AlgorithmFactories.Packing_BFD_v1,
	];
	
	private readonly AlgorithmFactory<IPackingAlgorithm>[] algorithmFactories_v2 =
	[
		AlgorithmFactories.Packing_FFD_v2,
		AlgorithmFactories.Packing_WFD_v2,
		AlgorithmFactories.Packing_BFD_v2,
	];

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public PackingResult ForLoop_v1()
	{
		var results = new PackingResult[this.algorithmFactories_v1.Length];
		for (var i = 0; i < this.algorithmFactories_v1.Length; i++)
		{
			var algorithmFactory = this.algorithmFactories_v1[i];
			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
			var result = algorithmInstance.Execute(new PackingParameters
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[i] = result;
		}

		return results
			.OrderByDescending(x => x.PackedBinVolumePercentage)
			.FirstOrDefault()!;
	}

	[Benchmark]
	[BenchmarkOrder(11)]
	public PackingResult ParallelFor_v1()
	{
		var results = new PackingResult[this.algorithmFactories_v1.Length];
		Parallel.For(0, this.algorithmFactories_v1.Length, index =>
		{
			var algorithmFactory = this.algorithmFactories_v1[index];
			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
			var result = algorithmInstance.Execute(new PackingParameters
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[index] = result;
		});
		return results
			.OrderByDescending(x => x.PackedBinVolumePercentage)
			.FirstOrDefault()!;
	}
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult ForLoop_v2()
	{
		var results = new PackingResult[this.algorithmFactories_v2.Length];
		for (var i = 0; i < this.algorithmFactories_v2.Length; i++)
		{
			var algorithmFactory = this.algorithmFactories_v2[i];
			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
			var result = algorithmInstance.Execute(new PackingParameters
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[i] = result;
		}

		return results
			.OrderByDescending(x => x.PackedBinVolumePercentage)
			.FirstOrDefault()!;
	}

	[Benchmark]
	[BenchmarkOrder(21)]
	public PackingResult ParallelFor_v2()
	{
		var results = new PackingResult[this.algorithmFactories_v2.Length];
		Parallel.For(0, this.algorithmFactories_v2.Length, index =>
		{
			var algorithmFactory = this.algorithmFactories_v2[index];
			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
			var result = algorithmInstance.Execute(new PackingParameters
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[index] = result;
		});
		return results
			.OrderByDescending(x => x.PackedBinVolumePercentage)
			.FirstOrDefault()!;
	}
}
