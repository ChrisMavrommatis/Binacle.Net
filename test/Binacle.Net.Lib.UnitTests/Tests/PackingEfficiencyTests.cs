﻿using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class PackingEfficiencyTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public PackingEfficiencyTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(ORLibraryScenarioTestDataProvider))]
	public void OR_Library_Packing_FFD_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(ORLibraryScenarioTestDataProvider))]
	public void OR_Library_Packing_FFD_v2(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(ORLibraryScenarioTestDataProvider))]
	public void OR_Library_Packing_WFD_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_WFD_v1, scenario);

	[Theory]
	[ClassData(typeof(ORLibraryScenarioTestDataProvider))]
	public void OR_Library_Packing_BFD_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_BFD_v1, scenario);


	private void RunPackingScenarioTest<TAlgorithm>(
		AlgorithmFactory<TAlgorithm> algorithmFactory,
		Scenario scenario
	)
		where TAlgorithm : class, IPackingAlgorithm
	{
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new Packing.Models.PackingParameters
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
				customMessage: $"Packed Bin Volume Percentage {result.PackedBinVolumePercentage} exceeded Max Potential Efficiency Percentage {scenarioResult.MaxPotentialPackingEfficiencyPercentage}"
				);
	}
}
