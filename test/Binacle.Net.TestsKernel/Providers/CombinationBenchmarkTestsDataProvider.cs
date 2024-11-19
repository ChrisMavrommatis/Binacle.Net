using System.Collections;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Providers;

public class CombinationBenchmarkTestsDataProvider: IEnumerable<object[]>
{
	public static Dictionary<string, bool> TestCases = new()
	{
		// Collection:Bin Name, Fits
		{ "Rectangular-Cuboids::Small", true },
		{ "Rectangular-Cuboids::Medium", true },
		{ "Rectangular-Cuboids::Large", true },
	};

	// Review: This is a placeholder for the actual data
	public static List<TestItem> Items =
	[
		
	];
	
	public static List<Scenario> Scenarios = [
		Scenario.Create("Small_DoesNotFit", "Rectangular-Cuboids::Small", Items, "BinaryDecision::DoesNotFit"),
		Scenario.Create("Medium_Fits", "Rectangular-Cuboids::Medium", Items, "BinaryDecision::Fits"),
		Scenario.Create("Large_Fits", "Rectangular-Cuboids::Large", Items, "BinaryDecision::Fits"),
	];
	public IEnumerator<object[]> GetEnumerator()
	{
		foreach(var scenario in Scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
