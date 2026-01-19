using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class MultipleBins_FittingAlgorithms : MultipleBinsBenchmarkBase
{
	private static readonly OperationParameters _parameters = new OperationParameters
	{
		Operation = AlgorithmOperation.Fitting,
	};
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> FFD_Loop()
	{
		var results = this.LoopProcessor.Process(Algorithm.FirstFitDecreasing, this.Bins, this.Items, _parameters);
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public IDictionary<string, OperationResult> FFD_Parallel()
	{
		var results = this.ParallelProcessor.Process(Algorithm.FirstFitDecreasing, this.Bins, this.Items, _parameters);
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> WFD_Loop()
	{
		var results = this.LoopProcessor.Process(Algorithm.WorstFitDecreasing, this.Bins, this.Items, _parameters);
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(21)]
	public IDictionary<string, OperationResult> WFD_Parallel()
	{
		var results = this.ParallelProcessor.Process(Algorithm.WorstFitDecreasing, this.Bins, this.Items, _parameters);
		
		return results;
	}

	
	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> BFD_Loop()
	{
		var results = this.LoopProcessor.Process(Algorithm.BestFitDecreasing, this.Bins, this.Items, _parameters);
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(31)]
	public IDictionary<string, OperationResult> BFD_Parallel()
	{
		var results = this.ParallelProcessor.Process(Algorithm.BestFitDecreasing, this.Bins, this.Items, _parameters);
		
		return results;
	}
}
