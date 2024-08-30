using Binacle.Net.TestsKernel.Models;
using System.Collections;

namespace Binacle.Net.TestsKernel.Providers;

public class BenchmarkScalingTestsDataProvider : IEnumerable<object[]>
{
	public const string BinCollectionName = "rectangular-cuboids";
	public static Dictionary<int, string> TestCases = new Dictionary<int, string>()
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

	private static IReadOnlyList<BenchmarkScalingScenario> scenarios = TestCases
		.Select(x => new BenchmarkScalingScenario(x.Key, x.Value))
		.ToList();

	// ranges for assertion 193-384, 385-576, 577-1000
	private static Dictionary<string, Models.Range> ranges = scenarios
		.GroupBy(x => x.ExpectedSize)
		.ToDictionary(
		g => g.FirstOrDefault().ExpectedSize,
		g => new Models.Range(g.Min(x => x.NoOfItems), g.Max(x => x.NoOfItems))
		);


	public static IEnumerable<int> GetNoOfItems()
	{
		return TestCases.Keys;
	}

	public static Dimensions GetDimensions() => new Dimensions(5, 5, 5);

	public IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public static void AssertSuccessfulResult(Lib.BinFittingOperationResult result, int noOfItems)
	{
		var foundBin = result.FoundBin?.ID ?? "None";

		if (!ranges.TryGetValue(foundBin, out var range))
		{
			throw new ApplicationException("Error. Uncaught Test Result");
		}

		if (!range.IsWithin(noOfItems))
		{
			throw new ApplicationException("Error. Invalid Test Result");
		}

	}

	public static void AssertSuccessfulResult(Lib.BinPackingResult result, int noOfItems)
	{
		var expectedSize = TestCases[noOfItems];
		var foundBin = expectedSize != "None" ? result.BinID : "None";

		if (!ranges.TryGetValue(foundBin, out var range))
		{
			throw new ApplicationException("Error. Uncaught Test Result");
		}

		if (!range.IsWithin(noOfItems))
		{
			throw new ApplicationException("Error. Invalid Test Result");
		}

	}
}
