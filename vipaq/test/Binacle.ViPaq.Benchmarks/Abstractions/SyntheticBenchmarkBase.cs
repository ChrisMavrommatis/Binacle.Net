using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;

namespace Binacle.ViPaq.Benchmarks.Abstractions;

// Fans a benchmark out over the synthetic scenarios (deterministic random, at item counts no real pack reaches).
// CPU and memory only — never size, which random data misreads. The synthetic encode and decode
// classes share this; only their [Benchmark] methods differ.
public abstract class SyntheticBenchmarkBase : ScenarioBenchmarkBase
{
	[ParamsSource(typeof(SyntheticDataProvider), nameof(SyntheticDataProvider.Names))]
	public override string ScenarioName { get; set; } = "";

	protected override Scenario GetScenario(string name)
		=> SyntheticDataProvider.GetByName(name);
}
