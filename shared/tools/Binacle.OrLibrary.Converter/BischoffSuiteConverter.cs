using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Geometry;
using Binacle.TestReporting;

namespace Binacle.OrLibrary.Converter;

// Converts the raw OR-Library text (thpack1..7, the Bischoff & Ratcliff instances) into the tests-kernel
// bischoff-suite JSON, one file per thpack. thpack8 and thpack9 are NOT part of the Bischoff suite - different
// sources, a different problem class - so they are left out.
//
// Metrics is pure arithmetic over the bin and box types, and Result is a fixed baseline, so the tool never runs
// the packer. Output is deterministic, so a no-change re-run is byte-identical.
public sealed class BischoffSuiteConverter : IConverter
{
	// Every Bischoff & Ratcliff instance fills the container to ~98% but never tessellates perfectly, so the
	// outcome is always PartiallyPacked. Recorded once per operation. The tests kernel runs the real packer
	// against this baseline, so a FullyPacked or NotPacked fails and signals a real change.
	private const string ExpectedResult = "PartiallyPacked PartiallyPacked";

	public void Convert()
	{
		var outputDir = RepositoryRoot.Bind().Find("shared", "data", "bischoff-suite");

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

	// Read from the tool's own embedded resources, so the input travels with the build.
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

	// "ItemsVolume BinVolume ItemsCount Percentage": totals over all box types, the bin's volume, and how full
	// it would be if every box fit. Percentage keeps 2 decimals with trailing zeros trimmed but one kept (98.83,
	// 98.7, 100.0). Volumes use long so the multiply cannot overflow mid-sum.
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
