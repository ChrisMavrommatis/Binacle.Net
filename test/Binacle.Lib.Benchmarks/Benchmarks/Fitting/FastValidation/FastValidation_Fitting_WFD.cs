using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_Fitting_WFD : FastValidatonBenchmarkBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult WFD_v1()
		=> this.Run(AlgorithmFactories.WFD_v1, AlgorithmOperation.Fitting);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult WFD_v2()
		=> this.Run(AlgorithmFactories.WFD_v2, AlgorithmOperation.Fitting);
}
