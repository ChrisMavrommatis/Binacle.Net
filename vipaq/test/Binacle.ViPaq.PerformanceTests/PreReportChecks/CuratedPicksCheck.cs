using Binacle.ViPaq.TestsKernel.Providers;

namespace Binacle.ViPaq.PerformanceTests.PreReportChecks;

// Confirms every curated benchmark pick still resolves to a real generated scenario, so a stale pick is caught
// here in a sentence rather than deep in a BenchmarkDotNet run.
internal sealed class CuratedPicksCheck : IPreReportCheck
{
	public void Run()
	{
		Assert(BischoffCuratedProvider.Names, BischoffDataProvider.Names, "Bischoff");
		Assert(CustomProblemsCuratedProvider.Names, CustomProblemsDataProvider.Names, "custom");
	}

	private static void Assert(IEnumerable<string> curated, IEnumerable<string> available, string family)
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
}
