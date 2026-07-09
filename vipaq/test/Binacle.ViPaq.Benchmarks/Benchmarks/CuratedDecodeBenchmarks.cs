using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;

// [REVIEW-VIPAQ_TEST]
// Decode cost over the curated scenarios: turning bytes back into a bin and items. Setup pre-encodes both forms
// once so the benchmark times only the read. Protobuf is the baseline row, ViPaq the ratio.
// The two overrides point this class at the curated set; an "all scenarios" class would repeat them with the full provider.
[MemoryDiagnoser]
public class CuratedDecodeBenchmarks : ScenarioBenchmarkBase
{
	private byte[] protobufBytes = [];
	private byte[] vipaqToken = [];

	[ParamsSource(typeof(CuratedScenarioProvider), nameof(CuratedScenarioProvider.GetScenarioNames))]
	public override string ScenarioName { get; set; } = "";

	protected override Scenario GetScenario(string name) => CuratedScenarioProvider.GetScenarioByName(name);

	public override void GlobalSetup()
	{
		base.GlobalSetup();
		this.protobufBytes = ProtobufCodec.Encode(this.Scenario);
		this.vipaqToken = ViPaqCodec.Encode(this.Scenario);
	}

	[Benchmark(Baseline = true)]
	public PackedResult Protobuf() => PackedResult.Parser.ParseFrom(this.protobufBytes);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) ViPaq() => ViPaqCodec.Decode(this.vipaqToken);
}
