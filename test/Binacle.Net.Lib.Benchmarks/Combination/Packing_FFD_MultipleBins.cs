using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class Packing_FFD_MultipleBins
{
	private readonly BinCollectionsTestDataProvider binCollectionsDataProvider;
	private readonly MultiBinsBenchmarkTestsDataProvider dataProvider;
	private readonly List<TestBin> bins;
	private readonly List<TestItem> items;
	public Packing_FFD_MultipleBins()
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider();
		this.dataProvider = new MultiBinsBenchmarkTestsDataProvider();
		this.bins = this.dataProvider.GetBins(this.binCollectionsDataProvider);
		this.items = this.dataProvider.GetItems();
	}

	[Params(1, 2)]
	public int AlgorithmVersion { get; set; }

	private readonly Dictionary<int, AlgorithmFactory<IPackingAlgorithm>> algorithmFactories = new()
	{
		{ 1, AlgorithmFactories.Packing_FFD_v1 },
		{ 2, AlgorithmFactories.Packing_FFD_v2 },
	};

	[Benchmark(Baseline = true)]
	public Dictionary<string, PackingResult> ForeachLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, PackingResult>();
		
		foreach(var bin in this.bins)
		{
			var algorithmInstance = algorithmFactory(bin, this.items);
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
		var results = new Dictionary<string, PackingResult>();
		
		Parallel.For(0, this.bins.Count, index =>
		{
			var bin = this.bins[index];
			var algorithmInstance = algorithmFactory(bin, this.items);
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
