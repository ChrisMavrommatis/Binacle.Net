using Binacle.Net.TestsKernel.Benchmarks;

namespace Binacle.Lib.UnitTests.Data.Providers.Benchmarks;

public class PackingCubeScalingBenchmarksProvider : CubeScalingBenchmarkTestsDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.PackingAlgorithms)
		{
			foreach (var (scenarioKey, scenario) in CubeScalingBenchmarkTestsDataProvider.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
