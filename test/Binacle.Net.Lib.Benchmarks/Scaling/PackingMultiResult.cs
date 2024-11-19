using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Scaling;


[MemoryDiagnoser]
public class PackingMultiResult : SingleScenarioScalingBase
{
	public PackingMultiResult() : base("Rectangular-Cuboids::Small")
	{
	}

	private readonly AlgorithmFactory<IPackingAlgorithm>[] algorithmFactories = [
    		AlgorithmFactories.Packing_FFD_v1,
    		AlgorithmFactories.Packing_FFD_v2,
    		AlgorithmFactories.Packing_WFD_v1,
    		AlgorithmFactories.Packing_FFD_v1,	
	];
	// private readonly AlgorithmFactory<IPackingAlgorithm>[] algorithmFactories = [
	// 	AlgorithmFactories.Packing_FFD_v1,
	// 	AlgorithmFactories.Packing_FFD_v1,
	// 	AlgorithmFactories.Packing_FFD_v1,
	// 	AlgorithmFactories.Packing_FFD_v1,
	// 	AlgorithmFactories.Packing_FFD_v1,
	// 	AlgorithmFactories.Packing_FFD_v1,
	// 	AlgorithmFactories.Packing_FFD_v1
	// ];

	[Benchmark(Baseline = true)]
	public PackingResult ForLoop()
	{
		var results = new PackingResult[this.algorithmFactories.Length];
		for(var i = 0; i< this.algorithmFactories.Length; i++)
		{
			var algorithmFactory = this.algorithmFactories[i];
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
	public PackingResult ParallelFor()
	{
		var results = new PackingResult[this.algorithmFactories.Length];
		Parallel.For(0, this.algorithmFactories.Length, index =>
		{
			var algorithmFactory = this.algorithmFactories[index];
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
