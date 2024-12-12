using Binacle.Net.TestsKernel.Models;
using System.Collections;

namespace Binacle.Net.TestsKernel.Providers;

public class ScalingBenchmarkTestsDataProvider : IEnumerable<object[]>
{
	// 1 min
	// 1 max
	// 1 over
	// and X in between
	public static int TestsPerCase = 2;

	public static Dictionary<string, ScalingBenchmarkTestCase> TestCases = new()
	{
		{ "Rectangular-Cuboids::Small", new("5x5x5", 10, 192) },
		{ "Rectangular-Cuboids::Medium", new("5x5x5", 10, 384) },
		{ "Rectangular-Cuboids::Large", new("5x5x5", 10, 576) },
	};

	public static Dictionary<string, ScalingBenchmarkScenario> Scenarios = TestCases
		.ToDictionary(x => x.Key, x => new ScalingBenchmarkScenario(x.Key, x.Value));

	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach(var (key, scenario) in Scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

