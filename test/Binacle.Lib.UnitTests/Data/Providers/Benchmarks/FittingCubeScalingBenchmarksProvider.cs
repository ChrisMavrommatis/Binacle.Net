using Binacle.Net.TestsKernel.Benchmarks;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Lib.UnitTests.Data.Providers.Benchmarks;

public class FittingCubeScalingBenchmarksProvider : CubeScalingBenchmarkTestsDataProvider
{
	public override IEnumerator<object[]> GetEnumerator()
	{
		foreach (var (algorithmKey, algorithm) in AlgorithmsUnderTest.FittingAlgorithms)
		{
			foreach (var (scenarioKey, scenario) in CubeScalingBenchmarkTestsDataProvider.Scenarios)
			{
				yield return new object[] { algorithmKey, scenario };
			}
		}
	}
}
