using Binacle.TestReporting;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.ViPaq;
using Microsoft.Extensions.Logging;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// Answers the open question O1: at what item count does compressing the token first pay off?
//
// It sweeps the real samples (the same ones the size report uses) ordered by item count, one width family
// at a time so it stays like-for-like. At each sample it sets the token ViPaq actually produced (compressed
// on its own once the body passes a fixed threshold) against the uncompressed size the same data would take.
// The uncompressed size is exact arithmetic from the format — fixed-width fields, so it is header bytes +
// count + bin + items, no guessing. The crossover is the first sample where compression both kicks in and
// the token actually gets smaller.
//
// Caveat: real data is not a clean ladder. It has gaps (custom packs jump from 16 to 100 items; Bischoff
// packs all land at 57+), so the crossover point is as fine as the data allows, not to the single item.
internal class CompressionCrossoverTest : ITest
{
	private readonly IReadOnlyCollection<PackingSample> samples;
	private readonly string label;
	private readonly ILogger<CompressionCrossoverTest> logger;

	public ResultFile File { get; }

	public CompressionCrossoverTest(
		IReadOnlyCollection<PackingSample> samples,
		string label,
		ResultFile file,
		ILogger<CompressionCrossoverTest> logger
	)
	{
		this.samples = samples;
		this.label = label;
		this.File = file;
		this.logger = logger;
	}

	public TestResult Run()
	{
		var table = new TableResult("Sample", "Items", "Widths b/i/c", "Raw b64", "Actual b64", "Comp", "Saved %");
		PackingSample? crossoverSample = null;

		var orderedSamples = this.samples
			.OrderBy(sample => sample.ItemCount)
			.ThenBy(sample => sample.Name);

		foreach (var sample in orderedSamples)
		{
			var token = ViPaqCodec.Encode(sample);
			var header = ViPaqHeader.Read(token);

			var actualBase64 = ViPaqCodec.ToBase64(token).Length;
			var rawBase64 = Base64Length(UncompressedBytes(header, sample.ItemCount));

			var compressed = header.IsCompressed;
			var savedPercent = compressed ? (rawBase64 - actualBase64) / (double)rawBase64 * 100 : (double?)null;

			if (crossoverSample is null && compressed && actualBase64 < rawBase64)
			{
				crossoverSample = sample;
			}

			table.AddRow(
				sample.Name,
				sample.ItemCount.ToString(),
				Widths(header),
				rawBase64.ToString(),
				actualBase64.ToString(),
				compressed ? "Y" : "N",
				savedPercent is null ? "-" : $"{savedPercent:F0}%");
		}

		this.logger.LogInformation(
			"{Label}: compression first pays off at {Where}.",
			this.label,
			crossoverSample is null
				? "(never in this set)"
				: $"{crossoverSample.ItemCount} items ({crossoverSample.Name})");

		return new TestResult
		{
			Title = $"Compression crossover — {this.label}",
			File = this.File,
			Description = crossoverSample is null
				? "Compression never beat the raw token in this set."
				: $"Compressed base64 first beats raw at {crossoverSample.ItemCount} items ({crossoverSample.Name}).",
			Result = table
		};
	}

	// Exact uncompressed size: 1 header byte + a 2-byte count + bin dimensions + per-item dimensions and
	// coordinates, each at the fixed width ViPaq chose (read from the header).
	private static int UncompressedBytes(ViPaqHeader header, int itemCount)
	{
		var binBytes = WidthBytes(header.BinDimensionsBitSize);
		var itemBytes = WidthBytes(header.ItemDimensionsBitSize);
		var coordinateBytes = WidthBytes(header.ItemCoordinatesBitSize);

		return 1
			+ 2
			+ (3 * binBytes)
			+ (itemCount * ((3 * itemBytes) + (3 * coordinateBytes)));
	}

	private static int Base64Length(int byteCount) => (byteCount + 2) / 3 * 4;

	private static int WidthBytes(BitSize bitSize) => bitSize switch
	{
		BitSize.Eight => 1,
		BitSize.Sixteen => 2,
		BitSize.ThirtyTwo => 4,
		BitSize.SixtyFour => 8,
		_ => throw new ArgumentOutOfRangeException(nameof(bitSize), bitSize, null)
	};

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
