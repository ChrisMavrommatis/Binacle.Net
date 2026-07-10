using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;

// Encode cost over the curated scenarios, uncompressed: turning a scenario into bytes. The codec is NoOp — the
// body is passed straight through — so this times the format alone, not compression. ViPaq is split into its two
// layouts, row-major and columnar, against the protobuf baseline. Compression time (deflate vs gzip) is a
// separate question, to be measured when the codec is raced on time.
[MemoryDiagnoser]
public class CuratedEncodeBenchmarks : ScenarioBenchmarkBase
{
	[ParamsSource(typeof(CuratedScenarioProvider), nameof(CuratedScenarioProvider.GetScenarioNames))]
	public override string ScenarioName { get; set; } = "";

	private ProtobufEncoder protobufEncoder = null!;
	private ViPaqEncoder vipaqEncoder = null!;

	protected override Scenario GetScenario(string name)
		=> CuratedScenarioProvider.GetScenarioByName(name);

	public override void GlobalSetup()
	{
		base.GlobalSetup();
		this.protobufEncoder = new ProtobufEncoder(new NoOpCodec());
		this.vipaqEncoder = new ViPaqEncoder(new NoOpCodec());
	}

	[Benchmark(Baseline = true)]
	public byte[] Protobuf()
		=> this.protobufEncoder.Encode(this.Scenario);

	[Benchmark]
	public byte[] ViPaq_Row()
		=> this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	public byte[] ViPaq_Column()
		=> this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.Columnar);
}
