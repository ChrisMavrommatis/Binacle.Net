using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Sanity Tests", "Ensures the tests are configured correctly")]
public class ScalingBenchmarksCaseTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public ScalingBenchmarksCaseTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(Data.Providers.Benchmarks.FittingScalingBenchmarksProvider))]
	public void Fitting_Algorithms(string algorithm, ScalingBenchmarkScenario scenario)
		=> this.RunFittingScenarioTest(algorithm, scenario);

	private void RunFittingScenarioTest(
		string algorithmKey,
		ScalingBenchmarkScenario scenario
	)
	{
		var algorithmFactory = this.Fixture.FittingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);
		foreach (var noOfItems in scenario.GetNoOfItems())
		{
			var items = scenario.GetTestItems(noOfItems);
			var algorithmInstance = algorithmFactory(bin, items);

			var result = algorithmInstance.Execute(new Fitting.Models.FittingParameters
				{ ReportFittedItems = false, ReportUnfittedItems = false });

			if (scenario.MaxInRange < noOfItems) // doesn't fit
			{
				Assert.Equal(Lib.Fitting.Models.FittingResultStatus.Fail, result.Status);
			}
			else
			{
				Assert.Equal(Lib.Fitting.Models.FittingResultStatus.Success, result.Status);
			}
		}
	}


	[Theory]
	[ClassData(typeof(Data.Providers.Benchmarks.PackingScalingBenchmarksProvider))]
	public void Packing_Algorithms(string algorithm, ScalingBenchmarkScenario scenario)
		=> this.RunPackingScenarioTest(algorithm, scenario);
	
	private void RunPackingScenarioTest(
		string algorithmKey,
		ScalingBenchmarkScenario scenario
	)
	{
		var algorithmFactory = this.Fixture.PackingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);
		foreach (var noOfItems in scenario.GetNoOfItems())
		{
			var items = scenario.GetTestItems(noOfItems);
			var algorithmInstance = algorithmFactory(bin, items);

			var result = algorithmInstance.Execute(new Packing.Models.PackingParameters
				{
					OptInToEarlyFails = true,
					NeverReportUnpackedItems = true,
					ReportPackedItemsOnlyWhenFullyPacked = true
				}
			);

			if (scenario.MaxInRange < noOfItems) // doesn't fit
			{
				Assert.NotEqual(Lib.Packing.Models.PackingResultStatus.FullyPacked, result.Status);
			}
			else
			{
				Assert.Equal(Lib.Packing.Models.PackingResultStatus.FullyPacked, result.Status);
			}
		}
	}
}
