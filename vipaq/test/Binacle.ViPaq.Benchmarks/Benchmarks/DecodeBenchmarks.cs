using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;
// [REVIEW-VIPAQ_TEST]
// Decode cost: turning bytes back into a bin and items. Setup pre-encodes both forms once so the benchmark
// times only the read. Protobuf is the baseline row, ViPaq the ratio.
[MemoryDiagnoser]
public class DecodeBenchmarks : SampleBenchmarkBase
{
	private byte[] protobufBytes = [];
	private byte[] vipaqToken = [];

	public override void GlobalSetup()
	{
		base.GlobalSetup();
		this.protobufBytes = ProtobufCodec.Encode(this.Sample);
		this.vipaqToken = ViPaqCodec.Encode(this.Sample);
	}

	[Benchmark(Baseline = true)]
	public PackedResult Protobuf() => PackedResult.Parser.ParseFrom(this.protobufBytes);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) ViPaq() => ViPaqCodec.Decode(this.vipaqToken);
}
