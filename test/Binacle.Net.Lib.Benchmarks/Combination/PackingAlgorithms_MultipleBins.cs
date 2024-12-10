using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class PackingAlgorithms_MultipleBins : MultipleBinsBenchmarkBase
{
	
	[Params("FFD v1", "WFD v1", "BFD v1")]
	public string AlgorithmVersion { get; set; }

	private readonly Dictionary<string, AlgorithmFactory<IPackingAlgorithm>> algorithmFactories = new()
	{
		{ "FFD v1", AlgorithmFactories.Packing_FFD_v1 },
		{ "WFD v1", AlgorithmFactories.Packing_WFD_v1 },
		{ "BFD v1", AlgorithmFactories.Packing_BFD_v1 }
	};

	[Benchmark(Baseline = true)]
	public Dictionary<string, PackingResult> ForeachLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, PackingResult>(this.Bins.Count);
		
		foreach(var bin in this.Bins)
		{
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new PackingParameters()
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[bin.ID] = result;
		}
		
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, PackingResult> ForLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, PackingResult>(this.Bins.Count);

		for (int i = 0; i < this.Bins.Count; i++)
		{
			var bin = this.Bins[i];
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new PackingParameters()
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[bin.ID] = result;
		}
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, PackingResult> SpanForLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, PackingResult>(this.Bins.Count);

		var binsSpan = CollectionsMarshal.AsSpan(this.Bins);

		for (int i = 0; i < binsSpan.Length; i++)
		{
			var bin = binsSpan[i];
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new PackingParameters()
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[bin.ID] = result;
		}
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, PackingResult> ParallelFor()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, PackingResult>(this.Bins.Count);
		
		Parallel.For(0, this.Bins.Count, index =>
		{
			var bin = this.Bins[index];
			var algorithmInstance = algorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new PackingParameters()
			{
				OptInToEarlyFails = false,
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			});
			results[bin.ID] = result;
		});

		return results;
	}
}
