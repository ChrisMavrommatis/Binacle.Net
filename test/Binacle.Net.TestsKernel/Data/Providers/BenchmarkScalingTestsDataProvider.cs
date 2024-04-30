using Binacle.Net.TestsKernel.Data.Models;
using Binacle.Net.TestsKernel.Models;
using System.Collections;

namespace Binacle.Net.TestsKernel.Data.Providers;

public class BenchmarkScalingTestsDataProvider : IEnumerable<object[]>
{
	public const string BinCollectionName = "RectangularCuboids";
	private static Dictionary<int, string> testCases = new Dictionary<int, string>()
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

	private static IReadOnlyList<BenchmarkScalingScenario> scenarios = testCases
		.Select(x => new BenchmarkScalingScenario(x.Key, x.Value))
		.ToList();

	// ranges for assertion 193-384, 385-576, 577-1000
	private static Dictionary<string, TestsKernel.Models.Range> ranges = scenarios
		.GroupBy(x => x.ExpectedSize)
		.ToDictionary(
		g => g.FirstOrDefault().ExpectedSize,
		g => new TestsKernel.Models.Range(g.Min(x => x.NoOfItems), g.Max(x => x.NoOfItems))
		);


	public static IEnumerable<int> GetNoOfItems()
	{
		return testCases.Keys;
	}

	public static Dimensions<int> GetDimensions() => new Dimensions<int>(5, 5, 5);

	public IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public static void AssertSuccessfulResult(Lib.Models.BinFittingOperationResult result, int noOfItems)
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
}
