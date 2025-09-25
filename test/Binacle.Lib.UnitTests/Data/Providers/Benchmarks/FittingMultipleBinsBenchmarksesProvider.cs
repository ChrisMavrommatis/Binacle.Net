using Binacle.Net.TestsKernel.Data.Providers.Benchmarks;

namespace Binacle.Lib.UnitTests.Data.Providers.Benchmarks;

public class FittingMultipleBinsBenchmarksesProvider : MultipleBinsBenchmarksDataProvider
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
