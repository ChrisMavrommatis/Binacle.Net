using BenchmarkDotNet.Attributes;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class MultipleItemsBenchmarkBase
{
	protected readonly BinCollectionsDataProvider BinCollectionsDataProvider;
	protected readonly MultipleBinsBenchmarksDataProvider DataProvider;
	protected readonly LoopBinProcessor LoopProcessor;
	protected readonly ParallelBinProcessor ParallelProcessor;
	public MultipleItemsBenchmarkBase()
	{
		this.BinCollectionsDataProvider = new BinCollectionsDataProvider();
		this.DataProvider = new MultipleBinsBenchmarksDataProvider();
		var algorithmFactory = new AlgorithmFactory();
		this.LoopProcessor = new LoopBinProcessor(algorithmFactory);
		this.ParallelProcessor = new ParallelBinProcessor(algorithmFactory);
		this.Bins = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.Take(2)
			.ToList();
	}
	
	/// 4,12, 24, 44, 58 
	[Params(1, 3, 5, 7, 9)]
	public int NoOfVariedItems { get; set; }
	public List<TestBin> Bins { get; set; }
	public List<TestItem> Items { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Items = this.DataProvider
			.GetItems()
			.Take(this.NoOfVariedItems)
			.ToList();
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.Items = new List<TestItem>();
	}
}
