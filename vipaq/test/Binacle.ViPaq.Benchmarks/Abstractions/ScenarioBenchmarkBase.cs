using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.Benchmarks.Abstractions;

// Fans the benchmarks out over a set of scenarios and hands each run its one scenario. The set is not fixed here:
// each benchmark class overrides ScenarioName (the names to run) and GetScenario (how to resolve one), so a
// curated class and a full "all scenarios" class share this base and only differ in the provider they point at.
public abstract class ScenarioBenchmarkBase
{
	public abstract string ScenarioName { get; set; }

	protected Scenario Scenario { get; private set; } = null!;

	protected abstract Scenario GetScenario(string name);

	[GlobalSetup]
	public virtual void GlobalSetup() => this.Scenario = this.GetScenario(this.ScenarioName);
}
