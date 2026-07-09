using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;
using Binacle.ViPaq.TestsKernel.ViPaq;

namespace Binacle.ViPaq.PerformanceTests.Tests;

// Fails the run early if the curated benchmark scenarios are wrong: a name that no longer resolves in the
// generated data, or a pick that takes the wrong path — an "uncompressed" one ViPaq compresses, or a
// "compressed" one it does not. Either way the benchmark's raw and gzip rows would be mislabelled. Cheaper to
// catch here than in a benchmark run.
internal static class CuratedScenarioCheck
{
	public static void Run()
	{
		AssertResolves(BischoffCuratedProvider.Names, BischoffDataProvider.Names, "Bischoff");
		AssertResolves(CustomProblemsCuratedProvider.Names, CustomProblemsDataProvider.Names, "custom");

		// Each pick must take the path its list claims, or the "raw" and "gzip" benchmark rows measure the wrong thing.
		AssertCompression(CustomProblemsCuratedProvider.UncompressedNames, CustomProblemsCuratedProvider.GetByName, shouldCompress: false);
		AssertCompression(CustomProblemsCuratedProvider.CompressedNames, CustomProblemsCuratedProvider.GetByName, shouldCompress: true);
		AssertCompression(BischoffCuratedProvider.Names, BischoffCuratedProvider.GetByName, shouldCompress: true);
	}

	private static void AssertResolves(IEnumerable<string> curated, IEnumerable<string> available, string family)
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

	private static void AssertCompression(IEnumerable<string> names, Func<string, Scenario> resolve, bool shouldCompress)
	{
		foreach (var name in names)
		{
			var token = ViPaqCodec.Encode(resolve(name));
			if (ViPaqHeader.Read(token).IsCompressed != shouldCompress)
			{
				var expected = shouldCompress ? "compressed" : "uncompressed";
				throw new InvalidOperationException(
					$"Curated scenario '{name}' is listed as {expected} but ViPaq did the opposite. " +
					"Move it to the other list or pick a different problem.");
			}
		}
	}
}
