using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_Packing_FFD : FastValidatonBenchmarkBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult FFD_v1()
		=> this.Run(AlgorithmFactories.FFD_v1, AlgorithmOperation.Packing);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult FFD_v2()
		=> this.Run(AlgorithmFactories.FFD_v2, AlgorithmOperation.Packing);
}
