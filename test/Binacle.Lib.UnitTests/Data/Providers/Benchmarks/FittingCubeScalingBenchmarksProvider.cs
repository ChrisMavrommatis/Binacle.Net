using Binacle.Net.TestsKernel.Data.Providers.Benchmarks;

namespace Binacle.Lib.UnitTests.Data.Providers.Benchmarks;

public class FittingCubeScalingBenchmarksProvider : CubeScalingBenchmarksDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.FittingAlgorithms)
		{
			foreach (var (scenarioKey, scenario) in CubeScalingBenchmarksDataProvider.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
