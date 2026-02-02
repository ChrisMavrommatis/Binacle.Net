using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks.Benchmarks.AlgorithmRacing;

[MemoryDiagnoser]
public class FFD_BFD_v1_Racing : AlgorithmRacingBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.RunLoop(
			[
				AlgorithmFactories.FFD_v1,
				AlgorithmFactories.BFD_v1,
			],
			AlgorithmOperation.Packing
		);
	
	[Benchmark]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> ParallelConcurrent()
		=> this.RunParallelConcurrent(
			[
				AlgorithmFactories.FFD_v1,
				AlgorithmFactories.BFD_v1,
			],
			AlgorithmOperation.Packing
		);
	
	[Benchmark]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> ParallelLock()
		=> this.RunParallelConcurrent(
			[
				AlgorithmFactories.FFD_v1,
				AlgorithmFactories.BFD_v1,
			],
			AlgorithmOperation.Packing
		);
}

[MemoryDiagnoser]
public class FFD_BFD_v2_Racing : AlgorithmRacingBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.RunLoop(
			[
				AlgorithmFactories.FFD_v2,
				AlgorithmFactories.BFD_v2,
			],
			AlgorithmOperation.Packing
		);
	
	[Benchmark]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> ParallelConcurrent()
		=> this.RunParallelConcurrent(
			[
				AlgorithmFactories.FFD_v2,
				AlgorithmFactories.BFD_v2,
			],
			AlgorithmOperation.Packing
		);
	
	[Benchmark]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> ParallelLock()
		=> this.RunParallelConcurrent(
			[
				AlgorithmFactories.FFD_v2,
				AlgorithmFactories.BFD_v2,
			],
			AlgorithmOperation.Packing
		);
}
