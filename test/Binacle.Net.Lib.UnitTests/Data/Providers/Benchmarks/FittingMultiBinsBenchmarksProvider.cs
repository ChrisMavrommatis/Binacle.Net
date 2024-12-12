using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers.Benchmarks;

public class FittingMultiBinsBenchmarksProvider : MultiBinsBenchmarkTestsDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.FittingAlgorithms)
		{
			foreach (var scenario in this.AllScenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
