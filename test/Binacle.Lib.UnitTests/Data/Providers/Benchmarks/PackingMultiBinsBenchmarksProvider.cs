using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Lib.UnitTests.Data.Providers.Benchmarks;

public class PackingMultiBinsBenchmarksProvider : MultiBinsBenchmarkTestsDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.PackingAlgorithms)
		{
			foreach (var scenario in this.AllScenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
