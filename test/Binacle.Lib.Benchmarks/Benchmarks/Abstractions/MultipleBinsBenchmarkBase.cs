using BenchmarkDotNet.Attributes;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class MultipleBinsBenchmarkBase
{
	protected readonly BinCollectionsDataProvider BinCollectionsDataProvider;
	protected readonly MultipleBinsBenchmarksDataProvider DataProvider;
	protected readonly LoopBinProcessor LoopProcessor;
	protected readonly ParallelBinProcessor ParallelProcessor;
	public MultipleBinsBenchmarkBase()
	{
		this.BinCollectionsDataProvider = new BinCollectionsDataProvider();
		this.DataProvider = new MultipleBinsBenchmarksDataProvider();
		var algorithmFactory = new AlgorithmFactory();
		this.LoopProcessor = new LoopBinProcessor(algorithmFactory);
		this.ParallelProcessor = new ParallelBinProcessor(algorithmFactory);
		this.Bins = new List<TestBin>();
		this.Items = this.DataProvider.GetItems();
	}
	
	[Params(2, 8, 14, 20, 26, 32, 38)]
	public int NoOfBins { get; set; }
	public List<TestBin> Bins { get; set; }
	public List<TestItem> Items { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Bins = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.Take(this.NoOfBins)
			.ToList();
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.Bins = new List<TestBin>();
	}
}
