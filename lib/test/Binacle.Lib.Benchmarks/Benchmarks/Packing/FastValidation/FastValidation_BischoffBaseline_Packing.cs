using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Lib.Benchmarks.FastValidation;

[MemoryDiagnoser]
public class FastValidation_BischoffBaseline_Packing : FastValidationBenchmarkBase
{
	protected override Scenario? GetScenario() =>
		BischoffSuiteScenarioProvider.GetScenarioByName(
			BischoffCuratedProblemsProvider.ScenarioDescriptions["Baseline"]
		);
	
	protected override AlgorithmOperation AlgorithmOperation 
		=> AlgorithmOperation.Packing;
}
