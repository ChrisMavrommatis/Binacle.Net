using Binacle.Net.TestsKernel.Models;
using System.Collections;

namespace Binacle.Net.TestsKernel.Providers;

public class BenchmarkScalingTestsDataProvider /*: IEnumerable<object[]>*/
{
	public static Dictionary<int, string> OldTestCases = new Dictionary<int, string>()
	{
		{ 10, "Small" },
		{ 50, "Small" },
		{ 100, "Small" },
		{ 190, "Small" },
		{ 192, "Small" },
		{ 193, "Medium" },
		{ 195, "Medium" },
		{ 300, "Medium" },
		{ 382, "Medium" },
		{ 384, "Medium" },
		{ 385, "Large" },
		{ 387, "Large" },
		{ 475, "Large" },
		{ 574, "Large" },
		{ 576, "Large" },
		{ 577, "None" },
		{ 580, "None" },
		{ 1000, "None" }
	};

	// 1 min
	// 1 max
	// 1 over
	// and X in between
	public static int TestsPerCase = 2;

	public static Dictionary<string, Models.BenchmarkTestCase> TestCases = new Dictionary<string, Models.BenchmarkTestCase>()
	{
		{ "Rectangular-Cuboids::Small", new("5x5x5", 10, 192) },
		{ "Rectangular-Cuboids::Medium", new("5x5x5", 10, 384) },
		{ "Rectangular-Cuboids::Large", new("5x5x5", 10, 576) },
	};

	public static Dictionary<string, BenchmarkScalingScenario> Scenarios = TestCases
		.ToDictionary(x => x.Key, x => new BenchmarkScalingScenario(x.Key, x.Value));


	//private static IReadOnlyList<BenchmarkScalingScenario> scenarios = TestCases
	//	.Select(x => new BenchmarkScalingScenario(x.Key, x.Value))
	//	.ToList();

	//// ranges for assertion 193-384, 385-576, 577-1000
	//private static Dictionary<string, Models.Range> ranges = scenarios
	//	.GroupBy(x => x.ExpectedSize)
	//	.ToDictionary(
	//	g => g.FirstOrDefault().ExpectedSize,
	//	g => new Models.Range(g.Min(x => x.NoOfItems), g.Max(x => x.NoOfItems))
	//	);


	//public static IEnumerable<int> GetNoOfItems()
	//{
	//	return TestCases.Keys;
	//}


	//public IEnumerator<object[]> GetEnumerator()
	//{
	//	foreach (var scenario in scenarios)
	//	{
	//		yield return new object[] { scenario };
	//	}
	//}

	//IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	//public static void AssertSuccessfulResult(Lib.Fitting.Models.FittingResult result, int noOfItems)
	//{
	//	// TODO: Implement this
	//	throw new NotImplementedException();
	//	//var foundBin = result.FoundBin?.ID ?? "None";

	//	//if (!ranges.TryGetValue(foundBin, out var range))
	//	//{
	//	//	throw new ApplicationException("Error. Uncaught Test Result");
	//	//}

	//	//if (!range.IsWithin(noOfItems))
	//	//{
	//	//	throw new ApplicationException("Error. Invalid Test Result");
	//	//}

	//}

	//public static void AssertSuccessfulResult(Lib.Packing.Models.PackingResult result, int noOfItems)
	//{
	//	// TODO: Implement this
	//	throw new NotImplementedException();

	//	//var expectedSize = TestCases[noOfItems];
	//	//var foundBin = expectedSize != "None" ? result.BinID : "None";

	//	//if (!ranges.TryGetValue(foundBin, out var range))
	//	//{
	//	//	throw new ApplicationException("Error. Uncaught Test Result");
	//	//}

	//	//if (!range.IsWithin(noOfItems))
	//	//{
	//	//	throw new ApplicationException("Error. Invalid Test Result");
	//	//}

	//}
}
