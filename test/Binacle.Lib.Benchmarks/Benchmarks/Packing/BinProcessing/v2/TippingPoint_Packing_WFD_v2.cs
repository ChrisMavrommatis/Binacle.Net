using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks.BinProcessing;

[MemoryDiagnoser]
public class TippingPoint_Packing_WFD_v2 : TippingPointBenchmarkBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.RunLoop(
			AlgorithmFactories.WFD_v2,
			AlgorithmOperation.Packing
		);

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> ParallelConcurrent()
		=> this.RunParallelConcurrent(
			AlgorithmFactories.WFD_v2,
			AlgorithmOperation.Packing
		);

	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> ParallelLock()
		=> this.RunParallelConcurrent(
			AlgorithmFactories.WFD_v2,
			AlgorithmOperation.Packing
		);
}
