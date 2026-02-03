using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks.AlgorithmRacing;

[MemoryDiagnoser]
public class FFD_BFD_WFD_Packing_Racing_v1 : AlgorithmRacingBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.RunLoop(
			[
				AlgorithmFactories.FFD_v1,
				AlgorithmFactories.BFD_v1,
				AlgorithmFactories.WFD_v1,
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
				AlgorithmFactories.WFD_v1,
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
				AlgorithmFactories.WFD_v1,
			],
			AlgorithmOperation.Packing
		);
}
