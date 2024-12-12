using System.Collections;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.TestsKernel.Providers;

public class MultiBinsBenchmarkTestsDataProvider : IEnumerable<object[]>
{
	private static readonly MultiBinsBenchmarkTestCase TestCase = new MultiBinsBenchmarkTestCase()
		.Add("Rectangular-Cuboids::Small", "BinaryDecision::DoesNotFit")  
		.Add("Rectangular-Cuboids::Medium", "BinaryDecision::Fits")
		.Add("Rectangular-Cuboids::Large", "BinaryDecision::Fits")			// 3 _ 2 
		
		.Add("Raw::60x40x10", "BinaryDecision::DoesNotFit")
		.Add("Raw::60x10x40", "BinaryDecision::DoesNotFit")
		.Add("Raw::40x60x10", "BinaryDecision::DoesNotFit")
		.Add("Raw::40x10x60", "BinaryDecision::DoesNotFit")
		.Add("Raw::10x40x60", "BinaryDecision::DoesNotFit")
		.Add("Raw::10x60x40", "BinaryDecision::DoesNotFit")					// 9 _ 2
		
		.Add("Raw::60x40x11", "BinaryDecision::DoesNotFit")
		.Add("Raw::60x11x40", "BinaryDecision::DoesNotFit")
		.Add("Raw::40x60x11", "BinaryDecision::DoesNotFit")
		.Add("Raw::40x11x60", "BinaryDecision::DoesNotFit")
		.Add("Raw::11x40x60", "BinaryDecision::DoesNotFit")
		.Add("Raw::11x60x40", "BinaryDecision::DoesNotFit")					// 15 _ 2
		
		.Add("Raw::60x40x12", "BinaryDecision::DoesNotFit")
		.Add("Raw::60x12x40", "BinaryDecision::DoesNotFit")
		.Add("Raw::40x60x12", "BinaryDecision::DoesNotFit")
		.Add("Raw::40x12x60", "BinaryDecision::DoesNotFit")
		.Add("Raw::12x40x60", "BinaryDecision::DoesNotFit")
		.Add("Raw::12x60x40", "BinaryDecision::DoesNotFit") 				// 21 _ 2
		
		.Add("Raw::60x40x20", "BinaryDecision::Fits")
		.Add("Raw::60x20x40", "BinaryDecision::Fits")
		.Add("Raw::40x60x20", "BinaryDecision::Fits")
		.Add("Raw::40x20x60", "BinaryDecision::Fits")
		.Add("Raw::20x40x60", "BinaryDecision::Fits")
		.Add("Raw::20x60x40", "BinaryDecision::Fits") 					    // 27 _ 8
		
		.Add("Raw::60x40x21", "BinaryDecision::Fits")
		.Add("Raw::60x21x40", "BinaryDecision::Fits")
		.Add("Raw::40x60x21", "BinaryDecision::Fits")
		.Add("Raw::40x21x60", "BinaryDecision::Fits")
		.Add("Raw::21x40x60", "BinaryDecision::Fits")
		.Add("Raw::21x60x40", "BinaryDecision::Fits") 						// 33 _ 14
		
		.Add("Raw::60x40x22", "BinaryDecision::Fits")
        .Add("Raw::60x22x40", "BinaryDecision::Fits")
        .Add("Raw::40x60x22", "BinaryDecision::Fits")
        .Add("Raw::40x22x60", "BinaryDecision::Fits")
        .Add("Raw::22x40x60", "BinaryDecision::Fits") 
        .Add("Raw::22x60x40", "BinaryDecision::Fits") 						// 39 _ 20
		
		.Add("Raw::60x40x30", "BinaryDecision::Fits")
		.Add("Raw::60x30x40", "BinaryDecision::Fits")
		.Add("Raw::40x60x30", "BinaryDecision::Fits")
		.Add("Raw::40x30x60", "BinaryDecision::Fits")
		.Add("Raw::30x40x60", "BinaryDecision::Fits")
		.Add("Raw::30x60x40", "BinaryDecision::Fits") 						// 45 _ 26
		
		.Add("Raw::60x40x31", "BinaryDecision::Fits")
		.Add("Raw::60x31x40", "BinaryDecision::Fits")
		.Add("Raw::40x60x31", "BinaryDecision::Fits")
		.Add("Raw::40x31x60", "BinaryDecision::Fits")
		.Add("Raw::31x40x60", "BinaryDecision::Fits")
		.Add("Raw::31x60x40", "BinaryDecision::Fits") 						// 51 _ 32
		
		.Add("Raw::60x40x32", "BinaryDecision::Fits")
		.Add("Raw::60x32x40", "BinaryDecision::Fits")
		.Add("Raw::40x60x32", "BinaryDecision::Fits")
		.Add("Raw::40x32x60", "BinaryDecision::Fits")
		.Add("Raw::32x40x60", "BinaryDecision::Fits")
		.Add("Raw::32x60x40", "BinaryDecision::Fits") 						// 57 _ 38
		;

	private static readonly ItemsCollection ItemsHolder = new ItemsCollection()
		.Add("10x10x40-4") // 4    1
		.Add("10x10x10-4") // 8
		.Add("5x5x5-4")    // 12   3
		.Add("5x10x10-7")  // 19 
		.Add("10x7x5-5")   // 24   5
		.Add("4x12x9-5")   // 29
		.Add("2x11x2-15")  // 44   7
		.Add("7x2x7-10")   // 54
		.Add("6x4x2-4");   // 58   9
		
	protected readonly Scenario[] AllScenarios = TestCase.GetScenarios(ItemsHolder).ToArray();
	
	public List<TestBin> GetAllBins(BinCollectionsTestDataProvider binCollectionsDataProvider)
	{
		var bins = new List<TestBin>();
		foreach (var scenario in this.AllScenarios)
		{
			var bin = scenario.GetTestBin(binCollectionsDataProvider);
			bins.Add(bin);
		}

		return bins;
	}
	
	protected readonly Scenario[] SuccessfulScenarios = TestCase.GetScenarios(ItemsHolder, x =>
	{
		if(x.ResultType != ScenarioResultType.BinaryDecision)
		{
			return false;
		}

		var result = x.ResultAs<BinaryDecisionScenarioResult>();
		return result.Fits;
	}).ToArray();
	
	public List<TestBin> GetSuccessfulBins(BinCollectionsTestDataProvider binCollectionsDataProvider)
	{
		var bins = new List<TestBin>();
		foreach (var scenario in this.SuccessfulScenarios)
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
	
	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach(var scenario in this.AllScenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
