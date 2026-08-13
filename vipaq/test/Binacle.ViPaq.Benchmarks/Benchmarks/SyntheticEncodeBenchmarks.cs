using BenchmarkDotNet.Attributes;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;

// Encode CPU/memory over the synthetic scenarios (2000 and 5000 items, 8- and 16-bit), against the protobuf
// baseline. The codec is NoOp — the body is passed straight through — so this times the format, not compression
// Run with `--filter *SyntheticEncode*`.
[MemoryDiagnoser]
public class SyntheticEncodeBenchmarks : SyntheticBenchmarkBase
{
	private ProtobufEncoder protobufEncoder = null!;
	private ViPaqEncoder vipaqEncoder = null!;

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
