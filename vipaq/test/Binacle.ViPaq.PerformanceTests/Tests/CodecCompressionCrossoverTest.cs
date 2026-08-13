using Binacle.TestReporting;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// Holds the layout fixed and puts the three codecs side by side, ViPaq only: for every scenario, the raw (NoOp)
// size next to the deflate and gzip sizes. That answers at a glance what the split size report buries — does
// compressing this pack pay at all, and which codec is smallest — and, read down the item-count ladder, where
// compression first starts to pay (the try-both threshold).
//
// One test is one (scenario set × layout); `Program` registers a Row file and a Columnar file. Rows are ordered
// by item count so the crossover is easy to spot. Real data is not a clean ladder — it has gaps — so the
// crossover is as fine as the data allows, not to the single item.
internal class CodecCompressionCrossoverTest : ITest
{
	private readonly IReadOnlyCollection<Scenario> scenarios;
	private readonly EncoderInfo encoderInfo;
	private readonly string title;
	private readonly string description;
	private readonly ILogger<CodecCompressionCrossoverTest> logger;

	public ResultFile File { get; }

	public CodecCompressionCrossoverTest(
		IReadOnlyCollection<Scenario> scenarios,
		EncoderInfo encoderInfo,
		string title,
		string description,
		ResultFile file,
		ILogger<CodecCompressionCrossoverTest> logger
	)
	{
		this.scenarios = scenarios;
		this.encoderInfo = encoderInfo;
		this.title = title;
		this.description = description;
		this.File = file;
		this.logger = logger;
	}

	public TestResult Run()
	{
		var table = new TableResult(
			"Scenario", "Items", "Widths b/i/c", "Raw b64", "Deflate b64", "Gzip b64", "Best", "Saved %");

		// Raw is NoOp: the body passed through, so it is the uncompressed baseline the other two are read against.
		var rawEncoder = new ViPaqEncoder(new NoOpCodec());
		var deflateEncoder = new ViPaqEncoder(new DeflateCodec());
		var gzipEncoder = new ViPaqEncoder(new GzipCodec());

		Scenario? crossover = null;

		// Order by item count (then name) so the ladder reads cleanly and the crossover below is the *smallest*
		// count where compression first pays. Unordered, the row order is embedded-resource enumeration order,
		// which is not stable across rebuilds.
		foreach (var scenario in this.scenarios
			.OrderBy(scenario => scenario.ItemCount)
			.ThenBy(scenario => scenario.Name, StringComparer.Ordinal))
		{
			var widths = ViPaqHeader.Create(scenario, this.encoderInfo).ToWidthsLabel();

			var raw = rawEncoder.Encode(scenario, this.encoderInfo).ToBase64().Length;
			var deflate = deflateEncoder.Encode(scenario, this.encoderInfo).ToBase64().Length;
			var gzip = gzipEncoder.Encode(scenario, this.encoderInfo).ToBase64().Length;

			// Savings of the better compressed form against raw. Negative when compression only inflated the pack.
			var bestCompressed = Math.Min(deflate, gzip);
			var savedPercent = (raw - bestCompressed) / (double)raw * 100;

			if (crossover is null && bestCompressed < raw)
			{
				crossover = scenario;
			}

			table.AddRow(
				scenario.Name,
				scenario.ItemCount.ToString(),
				widths,
				raw.ToString(),
				deflate.ToString(),
				gzip.ToString(),
				BestLabel(raw, deflate, gzip),
				$"{savedPercent:F0}%");
		}

		this.logger.LogInformation(
			"{Title}: compression first pays off at {Where}.",
			this.title,
			crossover is null
				? "(never in this set)"
				: $"{crossover.ItemCount} items ({crossover.Name})");

		return new TestResult
		{
			Title = this.title,
			File = this.File,
			Description = this.description,
			Result = table
		};
	}

	// The smallest of the three. Raw wins ties, then deflate — so a "Raw" best means compression did not pay here.
	private static string BestLabel(int raw, int deflate, int gzip)
	{
		if (raw <= deflate && raw <= gzip)
		{
			return "Raw";
		}

		return deflate <= gzip ? "Deflate" : "Gzip";
	}
}
