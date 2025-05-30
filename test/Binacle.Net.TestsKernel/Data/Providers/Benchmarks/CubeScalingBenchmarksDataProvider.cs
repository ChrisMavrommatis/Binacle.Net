using System.Collections;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Data.Providers.Benchmarks;

public class CubeScalingBenchmarksDataProvider : IEnumerable<object[]>
{
	// 1 min
	// 1 max
	// 1 over
	// and X in between
	public static int TestsPerCase = 2;

	public static Dictionary<string, CubeScalingBenchmarkTestCase> TestCases = new()
	{
		{ "Rectangular-Cuboids::Small", new("5x5x5", 10, 192) },
		{ "Rectangular-Cuboids::Medium", new("5x5x5", 10, 384) },
		{ "Rectangular-Cuboids::Large", new("5x5x5", 10, 576) },
	};

	public static Dictionary<string, CubeScalingBenchmarkScenario> Scenarios = TestCases
		.ToDictionary(x => x.Key, x => new CubeScalingBenchmarkScenario(x.Key, x.Value));

	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach(var (key, scenario) in Scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

