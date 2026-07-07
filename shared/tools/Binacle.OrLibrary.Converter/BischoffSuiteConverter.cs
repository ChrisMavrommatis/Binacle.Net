using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Geometry;

namespace Binacle.OrLibrary.Converter;

// Converts the raw OR-Library text (thpack1..7 — the Bischoff & Ratcliff instances) into the tests-kernel
// bischoff-suite JSON, one file per thpack. thpack8 and thpack9 are NOT part of the Bischoff suite (different
// sources and a different problem class) and are left out — see the raw data README.
//
// Metrics is pure arithmetic over the bin and box types. Result is a fixed baseline (see ExpectedResult below),
// so the tool never runs the packer and needs no algorithm dependency. Output is deterministic, so a no-change
// re-run is byte-identical (no git noise).
public sealed class BischoffSuiteConverter : IConverter
{
	// Every Bischoff & Ratcliff instance fills the container to ~98% but never tessellates perfectly, so the
	// outcome is always PartiallyPacked — some boxes go in, never all of them, and never none. We record that as
	// the expected result for both operations (packing and fitting) rather than running the packer here. The
	// tests kernel runs the real packer against this baseline and asserts they match: if an instance ever comes
	// out FullyPacked or NotPacked, that assertion fails — the signal that something packed unusually well, or
	// nothing fit at all.
	private const string ExpectedResult = "PartiallyPacked PartiallyPacked";

	public void Convert()
	{
		var outputDir = RepoLocator.FindOutputDir();

		var writeOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			// Content is plain ASCII (digits, 'x', brackets, spaces); the relaxed encoder leaves it unescaped.
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		};

		for (var thpack = 1; thpack <= 7; thpack++)
		{
			var thpackName = $"thpack{thpack}";
			var outputPath = Path.Combine(outputDir, $"orlib_{thpackName}.json");

			var problems = OrLibraryParser.Parse(ReadEmbeddedRawText(thpackName));
			var scenarios = problems.Select(problem => BuildScenario(thpackName, problem)).ToArray();

			// System.Text.Json omits the trailing newline; the committed files have one, so append it.
			var json = JsonSerializer.Serialize(scenarios, writeOptions) + "\n";
			File.WriteAllText(outputPath, json);

			Console.WriteLine($"Wrote {scenarios.Length} scenarios to {outputPath}");
		}
	}

	// Reads a raw thpack file from the tool's own embedded resources (LogicalName "thpackN.txt"), so the input
	// travels with the build and the tool doesn't have to locate the raw data on disk.
	private static string ReadEmbeddedRawText(string thpackName)
	{
		var resourceName = $"{thpackName}.txt";
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream(resourceName)
			?? throw new FileNotFoundException($"Embedded resource {resourceName} not found.");
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}

	private static Scenario BuildScenario(string thpackName, RawProblem problem)
	{
		var bin = new Dimensions<int>
		{
			Length = problem.BinLength,
			Width = problem.BinWidth,
			Height = problem.BinHeight,
		};

		return new Scenario
		{
			// The problem index within the file gives the stable name the tests already use.
			Name = $"OrLibrary_{thpackName}_{problem.Index}",
			Bin = CompactNotationFormatter.FormatDimensions(bin),
			Metrics = BuildMetrics(bin, problem.BoxTypes),
			Result = ExpectedResult,
			Items = problem.BoxTypes.Select(CompactNotationFormatter.FormatDimensionsAndQuantity).ToArray(),
		};
	}

	// "ItemsVolume BinVolume ItemsCount Percentage" — totals over all box types (volume and count), the bin's own
	// volume, and how full the bin would be if every box fit. Percentage is rounded to 2 decimals with trailing
	// zeros trimmed but at least one kept (e.g. 98.83, 98.7, 100.0). Volumes use long so the multiply can't
	// overflow mid-sum; the values themselves sit well within int.
	private static string BuildMetrics(Dimensions<int> bin, IReadOnlyList<RawBoxType> boxTypes)
	{
		var itemsVolume = boxTypes.Sum(box => (long)box.Length * box.Width * box.Height * box.Quantity);
		var binVolume = (long)bin.Length * bin.Width * bin.Height;
		var itemsCount = boxTypes.Sum(box => box.Quantity);

		var percentage = (decimal)itemsVolume / binVolume * 100m;
		var percentageText = Math.Round(percentage, 2, MidpointRounding.AwayFromZero)
			.ToString("0.0#", CultureInfo.InvariantCulture);

		return $"{itemsVolume} {binVolume} {itemsCount} {percentageText}";
	}
}
