using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;
// [REVIEW-VIPAQ_TEST]
// Encode cost: turning a sample into bytes. Protobuf is the baseline row, so ViPaq is reported as a ratio
// to it — that ratio stays comparable across machines and days even as raw numbers drift.
[MemoryDiagnoser]
public class EncodeBenchmarks : SampleBenchmarkBase
{
	[Benchmark(Baseline = true)]
	public byte[] Protobuf() => ProtobufCodec.Encode(this.Sample);

	[Benchmark]
	public byte[] ViPaq() => ViPaqCodec.Encode(this.Sample);
}
