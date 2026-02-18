using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.ScenarioProviders;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_BischoffBaseline_Packing : FastValidationBenchmarkBase
{
	protected override Scenario? GetScenario() =>
		BischoffSuiteScenarioProvider.GetScenarioByName("Baseline");
	
	protected override AlgorithmOperation AlgorithmOperation 
		=> AlgorithmOperation.Packing;
}
