using BenchmarkDotNet.Attributes;
using Binacle.Geometry;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;

// Decode CPU/memory over the synthetic scenarios, against the protobuf baseline. Setup pre-encodes each form once
// — plus the header ViPaq's decode needs — so only the read is timed. The codec is NoOp, so this times the format,
// not decompression. Run with `--filter *SyntheticDecode*`.
[MemoryDiagnoser]
public class SyntheticDecodeBenchmarks : SyntheticBenchmarkBase
{
	private ProtobufEncoder protobufEncoder = null!;
	private ViPaqEncoder vipaqEncoder = null!;
	private byte[] protobufToken = [];
	private byte[] vipaqTokenRow = [];
	private byte[] vipaqTokenCol = [];
	private ViPaqHeader vipaqHeaderRow;
	private ViPaqHeader vipaqHeaderCol;

	public override void GlobalSetup()
	{
		base.GlobalSetup();

		this.protobufEncoder = new ProtobufEncoder(new NoOpCodec());
		this.vipaqEncoder = new ViPaqEncoder(new NoOpCodec());

		this.protobufToken = this.protobufEncoder.Encode(this.Scenario);

		this.vipaqHeaderRow = ViPaqHeader.Create(this.Scenario, EncoderInfo.RowMajor);
		this.vipaqTokenRow = this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

		this.vipaqHeaderCol = ViPaqHeader.Create(this.Scenario, EncoderInfo.Columnar);
		this.vipaqTokenCol = this.vipaqEncoder.Encode(this.Scenario, EncoderInfo.Columnar);
	}

	[Benchmark(Baseline = true)]
	public PackedResult Protobuf()
		=> this.protobufEncoder.Decode(this.protobufToken);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) ViPaq_Row()
		=> this.vipaqEncoder.Decode(this.vipaqTokenRow, this.vipaqHeaderRow);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) ViPaq_Col()
		=> this.vipaqEncoder.Decode(this.vipaqTokenCol, this.vipaqHeaderCol);
}
