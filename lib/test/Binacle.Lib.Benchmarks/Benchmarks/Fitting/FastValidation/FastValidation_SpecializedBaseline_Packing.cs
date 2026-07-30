using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.TestsKernel.Algorithms.Models;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_SpecializedBaseline_Fitting : FastValidationBenchmarkBase
{
    protected override Scenario? GetScenario() =>
        SpecializedScalingProblemsProvider.GetBaseline();
	
    protected override AlgorithmOperation AlgorithmOperation 
        => AlgorithmOperation.Fitting;
}
