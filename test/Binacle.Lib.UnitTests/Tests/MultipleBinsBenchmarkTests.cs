using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;
using Binacle.Lib.UnitTests.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class MultipleBinsBenchmarkTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public MultipleBinsBenchmarkTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(FittingMultipleBinsBenchmarksProvider))]
	public void Fitting_Algorithms(string algorithm, Scenario scenario)
		=> this.RunFittingScenarioTest(algorithm, scenario);

	private void RunFittingScenarioTest(
		string algorithmKey,
		Scenario scenario
	)
	{
		var algorithmFactory = this.Fixture.FittingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new FittingParameters()
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			result.Status.ShouldBe(FittingResultStatus.Success);
		}
		else
		{
			result.Status.ShouldBe(FittingResultStatus.Fail);
		}
	}


	[Theory]
	[ClassData(typeof(PackingMultipleBinsBenchmarksProvider))]
	public void Packing_Algorithms(string algorithm, Scenario scenario)
		=> this.RunPackingScenarioTest(algorithm, scenario);

	private void RunPackingScenarioTest(
		string algorithmKey,
		Scenario scenario
	)
	{
		var algorithmFactory = this.Fixture.PackingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new PackingParameters
		{
			NeverReportUnpackedItems = true,
			ReportPackedItemsOnlyWhenFullyPacked = true,
			OptInToEarlyFails = true
		});

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			result.Status.ShouldBe(PackingResultStatus.FullyPacked);
		}
		else
		{
			result.Status.ShouldNotBe(PackingResultStatus.FullyPacked);
		}
	}

}
