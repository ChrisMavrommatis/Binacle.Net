using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.Lib.UnitTests.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class MultiBinsBenchmarkTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public MultiBinsBenchmarkTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(FittingMultiBinsBenchmarksProvider))]
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
			Assert.Equal(FittingResultStatus.Success, result.Status);
		}
		else
		{
			Assert.Equal(FittingResultStatus.Fail, result.Status);
		}
	}


	[Theory]
	[ClassData(typeof(PackingMultiBinsBenchmarksProvider))]
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
			Assert.Equal(PackingResultStatus.FullyPacked, result.Status);
		}
		else
		{
			Assert.NotEqual(PackingResultStatus.FullyPacked, result.Status);
		}
	}

}
