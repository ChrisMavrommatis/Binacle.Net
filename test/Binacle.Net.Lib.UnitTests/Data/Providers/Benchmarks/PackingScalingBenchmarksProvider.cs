using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers.Benchmarks;

public class PackingScalingBenchmarksProvider : ScalingBenchmarkTestsDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.PackingAlgorithms)
		{
			foreach (var (scenarioKey, scenario) in ScalingBenchmarkTestsDataProvider.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
