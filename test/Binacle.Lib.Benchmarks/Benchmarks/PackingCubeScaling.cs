using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;

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
		=> this.Run(AlgorithmFactories.Packing_FFD_v1, this.Bin!, this.Items!);
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public PackingResult FFD_v2()
		=> this.Run(AlgorithmFactories.Packing_FFD_v2, this.Bin!, this.Items!);

	
	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult WFD_v1()
		=> this.Run(AlgorithmFactories.Packing_WFD_v1, this.Bin!, this.Items!);
	
	[Benchmark]
	[BenchmarkOrder(21)]
	public PackingResult WFD_v2()
		=> this.Run(AlgorithmFactories.Packing_WFD_v2, this.Bin!, this.Items!);


	[Benchmark]
	[BenchmarkOrder(30)]
	public PackingResult BFD_v1()
		=> this.Run(AlgorithmFactories.Packing_BFD_v1, this.Bin!, this.Items!);
	
	[Benchmark]
	[BenchmarkOrder(31)]
	public PackingResult BFD_v2()
		=> this.Run(AlgorithmFactories.Packing_BFD_v2, this.Bin!, this.Items!);
	
	
	private PackingResult Run(
		AlgorithmFactory<IPackingAlgorithm> algorithmFactory,
		TestBin bin,
		List<TestItem> items
	)
	{
		var algorithmInstance = algorithmFactory(bin, items);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false,
			OptInToEarlyFails = true
		});
		return result;
	}
}
