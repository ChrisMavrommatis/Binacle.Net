using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Data.Providers.PackingEfficiency;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class PackingEfficiencyTests : IClassFixture<CommonTestingFixture>
{
	private readonly PackingAlgorithmFamiliesCollection algorithmFamilies;
	private CommonTestingFixture Fixture { get; }

	public PackingEfficiencyTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
		this.algorithmFamilies = new PackingAlgorithmFamiliesCollection();
	}

	[Theory]
	[ClassData(typeof(OrLibraryScenarioDataProvider))]
	public void OR_Library_Packing_FFD(Scenario scenario)
		=> this.RunPackingScenarioTests("FFD", scenario);

	[Theory]
	[ClassData(typeof(OrLibraryScenarioDataProvider))]
	public void OR_Library_Packing_WFD(Scenario scenario)
		=> this.RunPackingScenarioTests("WFD", scenario);

	[Theory]
	[ClassData(typeof(OrLibraryScenarioDataProvider))]
	public void OR_Library_Packing_BFD(Scenario scenario)
		=> this.RunPackingScenarioTests("BFD", scenario);


	private void RunPackingScenarioTests(
		string algorithmFamily,
		Scenario scenario
	)
	{
		var algorithms = this.algorithmFamilies[algorithmFamily];
		var results = new Dictionary<string, PackingResult>();
		foreach (var (algorithmKey, algorithmFactory) in algorithms)
		{
			var bin = scenario.GetTestBin(this.Fixture.BinDataProvider);
			var algorithmInstance = algorithmFactory(bin, scenario.Items);

			var result = algorithmInstance.Execute(new PackingParameters
			{
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false,
				OptInToEarlyFails = true
			});

			var scenarioResult = scenario.ResultAs<PackingEfficiencyScenarioResult>();

			var totalItems = result.PackedItems!.Count + result.UnpackedItems!.Count;
			totalItems.ShouldBe((int)scenarioResult.TotalItemCount);
			result.PackedBinVolumePercentage
				.ShouldBeLessThan(
					scenarioResult.MaxPotentialPackingEfficiencyPercentage,
					customMessage:
					$"Packed Bin Volume Percentage {result.PackedBinVolumePercentage} exceeded Max Potential Efficiency Percentage {scenarioResult.MaxPotentialPackingEfficiencyPercentage}"
				);

			results.Add(algorithmKey, result);
		}

		// All results should have same PackedBinVolumePercentage
		var firstResult = results.First();
		foreach (var (algorithm, result) in results.Skip(1))
		{
			result.PackedBinVolumePercentage
				.ShouldBe(
					firstResult.Value.PackedBinVolumePercentage,
					customMessage:
					$"Packed Bin Volume Percentage for algorithm {algorithm} ({result.PackedBinVolumePercentage}) does not match first result's algorithm {firstResult.Key} ({firstResult.Value.PackedBinVolumePercentage})"
				);
		}
	}
}
