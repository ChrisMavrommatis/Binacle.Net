using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;

// [REVIEW-VIPAQ_TEST]
// Encode cost over the curated scenarios: turning a scenario into bytes. Protobuf is the baseline row, so ViPaq
// is reported as a ratio to it — that ratio stays comparable across machines and days even as raw numbers drift.
// The two overrides point this class at the curated set; an "all scenarios" class would repeat them with the full provider.
[MemoryDiagnoser]
public class CuratedEncodeBenchmarks : ScenarioBenchmarkBase
{
	[ParamsSource(typeof(CuratedScenarioProvider), nameof(CuratedScenarioProvider.GetScenarioNames))]
	public override string ScenarioName { get; set; } = "";

	protected override Scenario GetScenario(string name) => CuratedScenarioProvider.GetScenarioByName(name);

	[Benchmark(Baseline = true)]
	public byte[] Protobuf() => ProtobufCodec.Encode(this.Scenario);

	[Benchmark]
	public byte[] ViPaq() => ViPaqCodec.Encode(this.Scenario);
}
