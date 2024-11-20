using System.Collections;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Providers;

public class MultiBinsBenchmarkTestsDataProvider : IEnumerable<object[]>
{
	private static readonly MultiBinsBenchmarkTestCase TestCase = new MultiBinsBenchmarkTestCase()
		.Add("Rectangular-Cuboids::Small", "BinaryDecision::DoesNotFit")
		.Add("Rectangular-Cuboids::Medium", "BinaryDecision::Fits")
		.Add("Rectangular-Cuboids::Large", "BinaryDecision::Fits");

	private static readonly ItemsCollection ItemsHolder = new ItemsCollection()
		.Add("10x10x40-2")
		.Add("10x10x10-4")
		.Add("5x5x5-4")
		.Add("5x10x10-3")
		.Add("10x7x5-2");
		

	private readonly Scenario[] scenarios = TestCase.GetScenarios(ItemsHolder).ToArray();
	public List<TestBin> GetBins(BinCollectionsTestDataProvider binCollectionsDataProvider)
	{
		var bins = new List<TestBin>();
		foreach (var scenario in this.scenarios)
		{
			var bin = scenario.GetTestBin(binCollectionsDataProvider);
			bins.Add(bin);
		}

		return bins;
	}
	public List<TestItem> GetItems()
	{
		return ItemsHolder.GetItems();
	}
	
	public IEnumerator<object[]> GetEnumerator()
	{
		foreach(var scenario in this.scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
