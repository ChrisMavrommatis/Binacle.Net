using BenchmarkDotNet.Attributes;
using Binacle.Geometry;
using Binacle.ViPaq.Benchmarks.Abstractions;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.Benchmarks.Benchmarks;

// Prices the compression itself, which the other benchmarks leave out by running NoOp only. NoOp passes the
// body straight through, so `Deflate − NoOp` is what deflate's squeezing costs and `Gzip − Deflate` is gzip's
// extra framing. Row-major, over the two curated Bischoff packs. Run with `--filter *CompressionCost*`.
[MemoryDiagnoser]
public class CompressionCostBenchmarks : ScenarioBenchmarkBase
{
	[ParamsSource(typeof(BischoffCuratedProvider), nameof(BischoffCuratedProvider.Names))]
	public override string ScenarioName { get; set; } = "";

	private ViPaqEncoder noopEncoder = null!;
	private ViPaqEncoder deflateEncoder = null!;
	private ViPaqEncoder gzipEncoder = null!;

	private ViPaqHeader header;
	private byte[] noopToken = [];
	private byte[] deflateToken = [];
	private byte[] gzipToken = [];

	protected override Scenario GetScenario(string name)
		=> BischoffCuratedProvider.GetByName(name);

	public override void GlobalSetup()
	{
		base.GlobalSetup();

		this.noopEncoder = new ViPaqEncoder(new NoOpCodec());
		this.deflateEncoder = new ViPaqEncoder(new DeflateCodec());
		this.gzipEncoder = new ViPaqEncoder(new GzipCodec());

		// One header, row-major with the compressed bit set; the codec decides whether the body is squeezed.
		this.header = ViPaqHeader.Create(this.Scenario, EncoderInfo.RowMajor);
		this.noopToken = this.noopEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
		this.deflateToken = this.deflateEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
		this.gzipToken = this.gzipEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);
	}

	[Benchmark(Baseline = true)]
	public byte[] Encode_NoOp()
		=> this.noopEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	public byte[] Encode_Deflate()
		=> this.deflateEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	public byte[] Encode_Gzip()
		=> this.gzipEncoder.Encode(this.Scenario, EncoderInfo.RowMajor);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Decode_NoOp()
		=> this.noopEncoder.Decode(this.noopToken, this.header);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Decode_Deflate()
		=> this.deflateEncoder.Decode(this.deflateToken, this.header);

	[Benchmark]
	public (Dimensions<ushort> Bin, IList<Item<ushort>> Items) Decode_Gzip()
		=> this.gzipEncoder.Decode(this.gzipToken, this.header);
}
