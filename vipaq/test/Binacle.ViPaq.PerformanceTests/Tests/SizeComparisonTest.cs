using Binacle.TestReporting;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Protobuf;
using Binacle.ViPaq.TestsKernel.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// [REVIEW-VIPAQ_TEST]
// The headline report: stored size for every scenario, ViPaq vs protobuf, plus the round-trip gate.
//
// The table shows raw byte counts first (ViPaq, proto, proto+gzip), then the base64 counts for the same
// three, so you can read both the on-the-wire bytes and the stored base64 text side by side and judge when
// each form is worth it. ViPaq bakes gzip into its own bytes (it compresses itself once the body passes a
// fixed threshold), so there is no separate "ViPaq gz" column — the "ViPaq comp" flag tells you when it
// kicked in. Protobuf is gzip'd on every row so you can see where compression starts to pay for it too.
//
// Two ratio columns, so ViPaq is compared against both protobuf forms:
//   - "ViPaq/Proto"    — ViPaq base64 vs raw protobuf base64. When ViPaq did not compress this is the
//                        like-for-like (both uncompressed) number; when it did, it shows the win over raw proto.
//   - "ViPaq/Proto gz" — ViPaq base64 vs gzip'd protobuf base64. The like-for-like number when ViPaq compressed;
//     blank ("-") otherwise, since raw ViPaq vs gzip-inflated proto is a baseline no one would use.
//
// A row that fails to round-trip is not a size win; it is flagged FAIL and logged as an error.
//
// The same test runs over more than one scenario set, so the set, title and description are passed in. Today that
// is the real custom and Bischoff packs; synthetic is a stub and, by D9, is never size-measured (gzip can't grip
// random data) — it only feeds the CPU/memory benchmarks.
internal class SizeComparisonTest : ITest
{
	private readonly IReadOnlyCollection<Scenario> scenarios;
	private readonly string title;
	private readonly string description;
	private readonly ILogger<SizeComparisonTest> logger;

	public ResultFile File { get; }

	public SizeComparisonTest(
		IReadOnlyCollection<Scenario> scenarios,
		string title,
		string description,
		ResultFile file,
		ILogger<SizeComparisonTest> logger
	)
	{
		this.scenarios = scenarios;
		this.title = title;
		this.description = description;
		this.File = file;
		this.logger = logger;
	}

	public TestResult Run()
	{
		var table = new TableResult(
			"Scenario", "Items", "Widths b/i/c", "ViPaq comp",
			"ViPaq bytes", "Proto bytes", "Proto gz bytes",
			"ViPaq b64", "Proto b64", "Proto gz b64",
			"ViPaq/Proto", "ViPaq/Proto gz", "Round-trip");

		var orderedScenarios = this.scenarios
			.OrderBy(scenario => scenario.WidthBits)
			.ThenBy(scenario => scenario.ItemCount)
			.ThenBy(scenario => scenario.Name);

		foreach (var scenario in orderedScenarios)
		{
			var token = ViPaqCodec.Encode(scenario);
			var header = ViPaqHeader.Read(token);
			var vipaqBase64 = ViPaqCodec.ToBase64(token);

			var protobufRaw = ProtobufCodec.Encode(scenario);
			var protobufBase64 = ProtobufCodec.ToBase64(protobufRaw);

			// Gzip protobuf on every row so the table shows where compression starts to pay for it.
			var protobufGzip = ProtobufCodec.EncodeGzip(scenario);
			var protobufGzipBase64 = ProtobufCodec.ToBase64(protobufGzip);

			// ViPaq vs raw proto is always shown. ViPaq vs gz proto only when ViPaq compressed; otherwise it would
			// compare a raw ViPaq token against gzip-inflated proto — no baseline. Blank ("-") on uncompressed rows.
			var ratioVsRaw = (double)vipaqBase64.Length / protobufBase64.Length;
			var ratioVsGzip = header.IsCompressed
				? $"{(double)vipaqBase64.Length / protobufGzipBase64.Length * 100:F0}%"
				: "-";

			var roundTrips = ViPaqCodec.RoundTrips(scenario, token);
			if (!roundTrips)
			{
				this.logger.LogError("Scenario {ScenarioName} did not round-trip.", scenario.Name);
			}

			table.AddRow(
				scenario.Name,
				scenario.ItemCount.ToString(),
				Widths(header),
				header.IsCompressed ? "Y" : "N",
				token.Length.ToString(),
				protobufRaw.Length.ToString(),
				protobufGzip.Length.ToString(),
				vipaqBase64.Length.ToString(),
				protobufBase64.Length.ToString(),
				protobufGzipBase64.Length.ToString(),
				$"{ratioVsRaw * 100:F0}%",
				ratioVsGzip,
				roundTrips ? "OK" : "FAIL");
		}

		return new TestResult
		{
			Title = this.title,
			File = this.File,
			Description = this.description,
			Result = table
		};
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
