using Binacle.ViPaq.Compression;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// The fail-fast gate that runs before the size report. It guards the one premise every number in that report
// rests on: a token is a size win only if it decodes back to the exact scenario it came from. So it encodes
// every real scenario in every mode the report measures — each codec, each layout — and decodes it straight
// back, throwing on the first mismatch. A bug in a layout or a codec surfaces here in a sentence, not as a
// wrong-but-plausible number buried in a table.
//
// It also confirms the curated benchmark picks still resolve, so a stale pick is caught here rather than deep in
// a BenchmarkDotNet run.
internal static class RoundTripCheck
{
	private static readonly ICompressionCodec[] Codecs =
		[new NoOpCodec(), new DeflateCodec(), new GzipCodec()];

	private static readonly EncoderInfo[] Layouts =
		[EncoderInfo.RowMajor, EncoderInfo.Columnar];

	public static void Run()
	{
		AssertCuratedResolves(BischoffCuratedProvider.Names, BischoffDataProvider.Names, "Bischoff");
		AssertCuratedResolves(CustomProblemsCuratedProvider.Names, CustomProblemsDataProvider.Names, "custom");

		AssertRoundTrips(CustomProblemsDataProvider.All);
		AssertRoundTrips(BischoffDataProvider.All);
	}

	private static void AssertRoundTrips(IReadOnlyCollection<Scenario> scenarios)
	{
		foreach (var codec in Codecs)
		{
			var encoder = new ViPaqEncoder(codec);
			foreach (var encoderInfo in Layouts)
			{
				foreach (var scenario in scenarios)
				{
					var header = ViPaqHeader.Create(scenario, encoderInfo);
					var token = encoder.Encode(scenario, encoderInfo);
					var (bin, items) = encoder.Decode(token, header);

					if (!Matches(scenario, bin, items))
					{
						throw new InvalidOperationException(
							$"Scenario '{scenario.Name}' did not round-trip "
							+ $"({codec.GetType().Name}, {encoderInfo.LayoutName}).");
					}
				}
			}
		}
	}

	private static void AssertCuratedResolves(IEnumerable<string> curated, IEnumerable<string> available, string family)
	{
		var have = available.ToHashSet();
		foreach (var name in curated)
		{
			if (!have.Contains(name))
			{
				throw new InvalidOperationException(
					$"Curated {family} scenario '{name}' is not in the generated data. Fix the pick or regenerate.");
			}
		}
	}

	private static bool Matches(Scenario scenario, IWithDimensions<ushort> bin, IList<Item<ushort>> items)
	{
		if (!SameDimensions(scenario.Bin, bin) || items.Count != scenario.Items.Length)
		{
			return false;
		}

		for (var index = 0; index < items.Count; index++)
		{
			var expected = scenario.Items[index];
			var actual = items[index];
			if (!SameDimensions(expected, actual) || !SameCoordinates(expected, actual))
			{
				return false;
			}
		}

		return true;
	}

	private static bool SameDimensions(IWithDimensions<ushort> left, IWithDimensions<ushort> right)
		=> left.Length == right.Length && left.Width == right.Width && left.Height == right.Height;

	private static bool SameCoordinates(IWithCoordinates<ushort> left, IWithCoordinates<ushort> right)
		=> left.X == right.X && left.Y == right.Y && left.Z == right.Z;
}
