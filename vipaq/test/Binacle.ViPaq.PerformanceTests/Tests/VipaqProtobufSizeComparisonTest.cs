using Binacle.TestReporting;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// Stored size for every scenario, ViPaq against protobuf, at one (codec, layout).
//
// One test is one table. `Program` registers one per (codec file × scenario set × layout), and protobuf runs the
// same codec as ViPaq, so the comparison is like-for-like — the only thing that differs is the format. That is
// the fairness rule the old report broke by comparing a compressed ViPaq token against raw protobuf.
//
// Base64 is the stored form, so it is the headline, with the raw byte counts beside it. There is no layout
// column: layout is fixed per table, and it does not move raw size anyway.
//
// The set, layout and codec are passed in. Today that is the real custom and Bischoff packs; synthetic is never
// size-measured (gzip can't grip random data) — it only feeds the BDN speed/memory benchmarks.
internal class VipaqProtobufSizeComparisonTest : ITest
{
	private readonly IReadOnlyCollection<Scenario> scenarios;
	private readonly ICompressionCodec compressionCodec;
	private readonly EncoderInfo encoderInfo;
	private readonly string title;
	private readonly string description;
	private readonly ILogger<VipaqProtobufSizeComparisonTest> logger;

	public ResultFile File { get; }

	public VipaqProtobufSizeComparisonTest(
		IReadOnlyCollection<Scenario> scenarios,
		ICompressionCodec compressionCodec,
		EncoderInfo encoderInfo,
		string title,
		string description,
		ResultFile file,
		ILogger<VipaqProtobufSizeComparisonTest> logger
	)
	{
		this.scenarios = scenarios;
		this.compressionCodec = compressionCodec;
		this.encoderInfo = encoderInfo;
		this.title = title;
		this.description = description;
		this.File = file;
		this.logger = logger;
	}

	public TestResult Run()
	{
		var table = new TableResult(
			"Scenario",
			"Items",
			"Widths b/i/c",
			"ViPaq bytes",
			"ViPaq b64",
			"Proto bytes",
			"Proto b64",
			"ViPaq/Proto"
		);

		var vipaqEncoder = new ViPaqEncoder(this.compressionCodec);
		var protobufEncoder = new ProtobufEncoder(this.compressionCodec);

		foreach (var scenario in this.scenarios)
		{
			var vipaqHeader = ViPaqHeader.Create(scenario, this.encoderInfo);
			var vipaqToken = vipaqEncoder.Encode(scenario, this.encoderInfo);
			var vipaqTokenBase64 = vipaqToken.ToBase64();

			// Same codec on protobuf as the ViPaq side, so the comparison measures format, not compression.
			var protobufToken = protobufEncoder.Encode(scenario);
			var protobufTokenBase64 = protobufToken.ToBase64();

			// Base64 against base64, the stored form, as a plain ratio: 0.30 means ViPaq is 30% the size of
			// protobuf. Under 1.0 means ViPaq is the smaller of the two.
			var ratio = (double)vipaqTokenBase64.Length / protobufTokenBase64.Length;

			table.AddRow(
				scenario.Name,
				scenario.ItemCount.ToString(),
				vipaqHeader.ToWidthsLabel(),
				vipaqToken.Length.ToString(),
				vipaqTokenBase64.Length.ToString(),
				protobufToken.Length.ToString(),
				protobufTokenBase64.Length.ToString(),
				$"{ratio:F2}"
			);
		}

		return new TestResult
		{
			Title = this.title,
			File = this.File,
			Description = this.description,
			Result = table
		};
	}
}
