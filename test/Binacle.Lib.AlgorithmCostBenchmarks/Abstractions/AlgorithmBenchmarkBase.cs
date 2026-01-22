using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.AlgorithmCostBenchmarks.Helpers;
using Binacle.Lib.AlgorithmCostBenchmarks.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.AlgorithmCostBenchmarks.Abstractions;

public abstract class AlgorithmBenchmarkBase
{
	// ==== Dimension 1: Number of Bins ====
	[Params(1, 2, 5, 10, 20, 50, 100)]
	public int BinCount { get; set; }
	
	// ==== Dimension 2: Number of Items ====
	[Params(10, 25, 50, 100, 250, 500, 1000)]
	public int ItemCount { get; set; }

	// ==== Dimension 3: Bin Size ====
	[Params(BinSize.Small, BinSize.Medium, BinSize.Large, BinSize.XLarge)]
	public BinSize BinSize { get; set; }
	
	// ==== Dimension 4: Item Size Distribution ====
	[Params(ItemSizeDistribution.Uniform, ItemSizeDistribution.Mixed, ItemSizeDistribution.MostlySmall, ItemSizeDistribution.MostlyLarge)]
	public ItemSizeDistribution ItemSizeDistribution { get; set; }
	
	// ==== Dimension 5: Execution Mode ====
	[Params(ExecutionMode.Loop, ExecutionMode.Parallel)]
	public ExecutionMode ExecutionMode { get; set; }
	
	private TestBin[] bins;
	private TestItem[] items;
	private LoopBinProcessor? loopProcessor;
	private ParallelBinProcessor? parallelProcessor;

	private const int RandomSeed = 596333;

	[GlobalSetup]
	public void GlobalSetup()
	{
		var algorithmFactory = GetAlgorithmFactory();
		this.loopProcessor = new LoopBinProcessor(algorithmFactory);
		this.parallelProcessor = new ParallelBinProcessor(algorithmFactory);
        
		var random = new Random(RandomSeed); // Fixed seed for reproducibility
		this.bins = ConstructionHelper.GenerateBins(random, this.BinCount, this.BinSize);
		this.items = ConstructionHelper.GenerateItems(random, this.ItemCount, this.ItemSizeDistribution, this.BinSize);

	}

	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.bins = [];
		this.items = [];
		this.loopProcessor = null;
		this.parallelProcessor = null;
	}
	
	protected abstract IAlgorithmFactory GetAlgorithmFactory();
	protected abstract Algorithm GetAlgorithm();
	
	[Benchmark]
	public IDictionary<string, OperationResult> Process()
	{
		var algorithm = this.GetAlgorithm();
		if (this.ExecutionMode == ExecutionMode.Loop)
		{
			return this.loopProcessor!.Process(
				algorithm,
				this.bins,
				this.items,
				new OperationParameters
				{
					Operation = AlgorithmOperation.Packing
				});
		}
		else
		{
			return this.parallelProcessor!.Process(
				algorithm,
				this.bins,
				this.items,
				new OperationParameters
				{
					Operation = AlgorithmOperation.Packing
				});
			
		}
	}
}
