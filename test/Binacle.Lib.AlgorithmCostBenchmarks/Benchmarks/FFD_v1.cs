using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;

namespace Binacle.Lib.AlgorithmCostBenchmarks.Benchmarks;

[MemoryDiagnoser]
public class FFD_v1 : Abstractions.AlgorithmBenchmarkBase
{
	protected override IAlgorithmFactory GetAlgorithmFactory()
		=> new V1_AlgorithmFactory();

	protected override Algorithm GetAlgorithm() 
		=> Algorithm.FirstFitDecreasing;
}

[MemoryDiagnoser]
public class BFD_v1 : Abstractions.AlgorithmBenchmarkBase
{
    protected override IAlgorithmFactory GetAlgorithmFactory()
        => new V1_AlgorithmFactory();

    protected override Algorithm GetAlgorithm()
        => Algorithm.BestFitDecreasing;
}

[MemoryDiagnoser]
public class WFD_v1 : Abstractions.AlgorithmBenchmarkBase
{
	protected override IAlgorithmFactory GetAlgorithmFactory()
		=> new V1_AlgorithmFactory();

	protected override Algorithm GetAlgorithm()
		=> Algorithm.WorstFitDecreasing;
}

[MemoryDiagnoser]
public class FFD_v2 : Abstractions.AlgorithmBenchmarkBase
{
	protected override IAlgorithmFactory GetAlgorithmFactory()
		=> new V2_AlgorithmFactory();

	protected override Algorithm GetAlgorithm()
		=> Algorithm.FirstFitDecreasing;
}

[MemoryDiagnoser]
public class BFD_v2 : Abstractions.AlgorithmBenchmarkBase
{
	protected override IAlgorithmFactory GetAlgorithmFactory()
		=> new V2_AlgorithmFactory();

	protected override Algorithm GetAlgorithm()
		=> Algorithm.BestFitDecreasing;
}

[MemoryDiagnoser]
public class WFD_v2 : Abstractions.AlgorithmBenchmarkBase
{
	protected override IAlgorithmFactory GetAlgorithmFactory()
		=> new V2_AlgorithmFactory();

	protected override Algorithm GetAlgorithm()
		=> Algorithm.WorstFitDecreasing;
}
