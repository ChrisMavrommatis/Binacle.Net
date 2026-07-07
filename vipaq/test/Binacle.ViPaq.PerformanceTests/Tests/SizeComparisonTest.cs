using Binacle.TestReporting;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// The headline report: stored size for every sample, ViPaq vs protobuf, plus the round-trip gate.
//
// The table shows raw byte counts first (ViPaq, proto, proto+gzip), then the base64 counts for the same
// three, so you can read both the on-the-wire bytes and the stored base64 text side by side and judge when
// each form is worth it. ViPaq bakes gzip into its own bytes (it compresses itself once the body passes a
// fixed threshold), so there is no separate "ViPaq gz" column — the "ViPaq comp" flag tells you when it
// kicked in. Protobuf is gzip'd on every row so you can see where compression starts to pay for it too.
//
// Fairness rule (from the plan): the ViPaq/Proto ratio scores ViPaq against the honest baseline — gzip'd
// protobuf when ViPaq itself compressed, raw protobuf when it did not — so the ratio always compares like
// against like even though both proto forms are always shown.
//
// A row that fails to round-trip is not a size win; it is flagged FAIL and logged as an error.
//
// The same test runs over more than one sample set (synthetic, and real data captured from the API), so
// the set, title and description are passed in. For real samples the SourceToken is the API's own token:
// we re-encode here and compare, as a cross-check that we produce the same bytes — never as the size.
internal class SizeComparisonTest : ITest
{
	private readonly IReadOnlyCollection<PackingSample> samples;
	private readonly string title;
	private readonly string description;
	private readonly ILogger<SizeComparisonTest> logger;

	public ResultFile File { get; }

	public SizeComparisonTest(
		IReadOnlyCollection<PackingSample> samples,
		string title,
		string description,
		ResultFile file,
		ILogger<SizeComparisonTest> logger
	)
	{
		this.samples = samples;
		this.title = title;
		this.description = description;
		this.File = file;
		this.logger = logger;
	}

	public TestResult Run()
	{
		var table = new TableResult(
			"Sample", "Items", "Widths b/i/c", "ViPaq comp",
			"ViPaq bytes", "Proto bytes", "Proto gz bytes",
			"ViPaq b64", "Proto b64", "Proto gz b64",
			"ViPaq/Proto", "Round-trip", "vs API");

		var orderedSamples = this.samples
			.OrderBy(sample => sample.WidthBits)
			.ThenBy(sample => sample.ItemCount)
			.ThenBy(sample => sample.Name);

		foreach (var sample in orderedSamples)
		{
			var token = ViPaqCodec.Encode(sample);
			var header = ViPaqHeader.Read(token);
			var vipaqBase64 = ViPaqCodec.ToBase64(token);

			var protobufRaw = ProtobufCodec.Encode(sample);
			var protobufBase64 = ProtobufCodec.ToBase64(protobufRaw);

			// Gzip protobuf on every row so the table shows where compression starts to pay for it.
			var protobufGzip = ProtobufCodec.EncodeGzip(sample);
			var protobufGzipBase64 = ProtobufCodec.ToBase64(protobufGzip);

			// Fairness rule: score ViPaq against gzip'd protobuf only when ViPaq itself compressed; else raw.
			var fairBaselineBase64 = header.IsCompressed ? protobufGzipBase64.Length : protobufBase64.Length;
			var ratio = (double)vipaqBase64.Length / fairBaselineBase64;

			var roundTrips = ViPaqCodec.RoundTrips(sample, token);
			if (!roundTrips)
			{
				this.logger.LogError("Sample {SampleName} did not round-trip.", sample.Name);
			}

			table.AddRow(
				sample.Name,
				sample.ItemCount.ToString(),
				Widths(header),
				header.IsCompressed ? "Y" : "N",
				token.Length.ToString(),
				protobufRaw.Length.ToString(),
				protobufGzip.Length.ToString(),
				vipaqBase64.Length.ToString(),
				protobufBase64.Length.ToString(),
				protobufGzipBase64.Length.ToString(),
				$"{ratio * 100:F0}%",
				roundTrips ? "OK" : "FAIL",
				ValidateAgainstSource(sample, vipaqBase64));
		}

		return new TestResult
		{
			Title = this.title,
			File = this.File,
			Description = this.description,
			Result = table
		};
	}

	// Cross-check against the source token, when there is one. MATCH means our re-encode is byte-identical
	// to what the API emitted; a Δ shows the base64-length difference (e.g. the API serialises from Int32,
	// so its count field can differ) — informational, not a pass/fail.
	private static string ValidateAgainstSource(PackingSample sample, string vipaqBase64)
	{
		if (string.IsNullOrEmpty(sample.SourceToken))
		{
			return "-";
		}

		if (string.Equals(sample.SourceToken, vipaqBase64, StringComparison.Ordinal))
		{
			return "MATCH";
		}

		var delta = vipaqBase64.Length - sample.SourceToken.Length;
		return $"Δ{delta:+0;-0}";
	}

	private static string Widths(ViPaqHeader header)
		=> $"{Bits(header.BinDimensionsBitSize)}/{Bits(header.ItemDimensionsBitSize)}/{Bits(header.ItemCoordinatesBitSize)}";

	private static string Bits(BitSize bitSize) => bitSize switch
	{
		BitSize.Eight => "8",
		BitSize.Sixteen => "16",
		BitSize.ThirtyTwo => "32",
		BitSize.SixtyFour => "64",
		_ => "?"
	};
}
