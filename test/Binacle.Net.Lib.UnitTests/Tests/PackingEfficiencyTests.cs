using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Models;
using Xunit;
using Xunit.Abstractions;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class PackingEfficiencyTests : IClassFixture<CommonTestingFixture>
{
	private readonly ITestOutputHelper outputHelper;

	private CommonTestingFixture Fixture { get; }
	public PackingEfficiencyTests(
		CommonTestingFixture fixture,
		ITestOutputHelper outputHelper
		)
	{
		this.Fixture = fixture;
		this.outputHelper = outputHelper;
	}

	[Theory]
	[ClassData(typeof(Data.Providers.PackingEfficiency.ORLibraryScenarioTestDataProvider))]
	public void OR_Library_v1(Scenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	private void RunPackingScenarioTest<TAlgorithm>(
			Func<TestBin, List<TestItem>, TAlgorithm> algorithmFactory,
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

		Xunit.Assert.Equal(scenarioResult.TotalItemCount, result.PackedItems.Count + result.UnpackedItems.Count);
		Xunit.Assert.True(
			scenarioResult.MaxPotentialPackingEfficiencyPercentage > result.PackedBinVolumePercentage, 
			$"Packed Bin Volume Percentage {result.PackedBinVolumePercentage} exceeded Max Potential Efficiency Percentage {scenarioResult.MaxPotentialPackingEfficiencyPercentage}"
		);

		var log = string.Format(
			"{0} = {1}/{2} = {3}",
			scenario.Name,
			result.PackedBinVolumePercentage, scenarioResult.MaxPotentialPackingEfficiencyPercentage,
			result.PackedBinVolumePercentage / scenarioResult.MaxPotentialPackingEfficiencyPercentage
			);

		this.outputHelper.WriteLine(log);
	}
}
