using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;
using Binacle.Lib.UnitTests.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Benchmarks.Models;

namespace Binacle.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class CubeScalingBenchmarksCaseTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public CubeScalingBenchmarksCaseTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(FittingCubeScalingBenchmarksProvider))]
	public void Fitting_Algorithms(string algorithm, CubeScalingBenchmarkScenario scenario)
		=> this.RunFittingScenarioTest(algorithm, scenario);

	private void RunFittingScenarioTest(
		string algorithmKey,
		CubeScalingBenchmarkScenario scenario
	)
	{
		var algorithmFactory = this.Fixture.FittingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);
		foreach (var noOfItems in scenario.GetNoOfItems())
		{
			var items = scenario.GetTestItems(noOfItems);
			var algorithmInstance = algorithmFactory(bin, items);

			var result = algorithmInstance.Execute(new FittingParameters
				{ ReportFittedItems = false, ReportUnfittedItems = false });

			if (scenario.MaxInRange < noOfItems) // doesn't fit
			{
				result.Status.ShouldBe(FittingResultStatus.Fail);
			}
			else
			{
				result.Status.ShouldBe(FittingResultStatus.Success);
			}
		}
	}


	[Theory]
	[ClassData(typeof(PackingCubeScalingBenchmarksProvider))]
	public void Packing_Algorithms(string algorithm, CubeScalingBenchmarkScenario scenario)
		=> this.RunPackingScenarioTest(algorithm, scenario);
	
	private void RunPackingScenarioTest(
		string algorithmKey,
		CubeScalingBenchmarkScenario scenario
	)
	{
		var algorithmFactory = this.Fixture.PackingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);
		foreach (var noOfItems in scenario.GetNoOfItems())
		{
			var items = scenario.GetTestItems(noOfItems);
			var algorithmInstance = algorithmFactory(bin, items);

			var result = algorithmInstance.Execute(new PackingParameters
				{
					OptInToEarlyFails = true,
					NeverReportUnpackedItems = true,
					ReportPackedItemsOnlyWhenFullyPacked = true
				}
			);

			if (scenario.MaxInRange < noOfItems) // doesn't fit
			{
				result.Status.ShouldNotBe(PackingResultStatus.FullyPacked);
			}
			else
			{
				result.Status.ShouldBe(PackingResultStatus.FullyPacked);
			}
		}
	}
}
