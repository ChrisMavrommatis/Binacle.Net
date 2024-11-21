using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class Fitting_FFD_MultipleBins
{
	private readonly BinCollectionsTestDataProvider binCollectionsDataProvider;
	private readonly MultiBinsBenchmarkTestsDataProvider dataProvider;
	private readonly List<TestBin> bins;
	private readonly List<TestItem> items;
	public Fitting_FFD_MultipleBins()
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider();
		this.dataProvider = new MultiBinsBenchmarkTestsDataProvider();
		this.bins = this.dataProvider.GetBins(this.binCollectionsDataProvider);
		this.items = this.dataProvider.GetItems();
	}

	[Params(1, 2, 3)]
	public int AlgorithmVersion { get; set; }

	private readonly Dictionary<int, AlgorithmFactory<IFittingAlgorithm>> algorithmFactories = new()
	{
		{ 1, AlgorithmFactories.Fitting_FFD_v1 },
		{ 2, AlgorithmFactories.Fitting_FFD_v2 },
		{ 3, AlgorithmFactories.Fitting_FFD_v3 },
	};

	[Benchmark(Baseline = true)]
	public Dictionary<string, FittingResult> ForeachLoop()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, FittingResult>(this.bins.Count);
		
		foreach(var bin in this.bins)
		{
			var algorithmInstance = algorithmFactory(bin, this.items);
			var result = algorithmInstance.Execute(new FittingParameters
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[bin.ID] = result;
		}
		 
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, FittingResult> ParallelFor()
	{
		var algorithmFactory = this.algorithmFactories[this.AlgorithmVersion];
		var results = new Dictionary<string, FittingResult>(this.bins.Count);
		
		Parallel.For(0, this.bins.Count, index =>
		{
			var bin = this.bins[index];
			var algorithmInstance = algorithmFactory(bin, this.items);
			var result = algorithmInstance.Execute(new FittingParameters
			{
				ReportFittedItems = false,
				ReportUnfittedItems = false
			});
			results[bin.ID] = result;
		});

		return results;
	}
}
