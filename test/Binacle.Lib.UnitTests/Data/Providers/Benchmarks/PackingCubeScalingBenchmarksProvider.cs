using Binacle.Net.TestsKernel.Data.Providers.Benchmarks;

namespace Binacle.Lib.UnitTests.Data.Providers.Benchmarks;

public class PackingCubeScalingBenchmarksProvider : CubeScalingBenchmarksDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.PackingAlgorithms)
		{
			foreach (var (scenarioKey, scenario) in CubeScalingBenchmarksDataProvider.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
