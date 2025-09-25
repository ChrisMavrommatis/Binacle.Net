using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class PackingMultiAlgorithm : MultipleBinsBenchmarkBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, PackingResult[]> Loop()
	{
		var results = this.LoopProcessor.ProcessPacking(this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public IDictionary<string, PackingResult[]> Parallel()
	{
		var results = this.ParallelProcessor.ProcessPacking(this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
}
