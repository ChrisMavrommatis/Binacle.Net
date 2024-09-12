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
		Fixture = fixture;
	}

	#region Fitting
	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Fitting_FFD_v1(BenchmarkScalingScenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Fitting_FFD_v2(BenchmarkScalingScenario scenario)
		=> this.RunFittingScenarioTest(AlgorithmFactories.Fitting_FFD_v2, scenario);

	private void RunFittingScenarioTest(
		Func<TestBin, List<TestItem>, IFittingAlgorithm> algorithmFactory,
		BenchmarkScalingScenario scenario
		)
	{
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);
		foreach (var noOfItems in scenario.GetNoOfItems())
		{
			var items = scenario.GetTestItems(noOfItems);
			var algorithmInstance = algorithmFactory(bin, items);

			var result = algorithmInstance.Execute(new Fitting.Models.FittingParameters { ReportFittedItems = false, ReportUnfittedItems = false });

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
	#endregion

	#region Packing

	[Theory]
	[ClassData(typeof(BenchmarkScalingTestsDataProvider))]
	public void Packing_FFD_v2(BenchmarkScalingScenario scenario)
		=> this.RunPackingScenarioTest(AlgorithmFactories.Packing_FFD_v1, scenario);

	private void RunPackingScenarioTest(
		Func<TestBin, List<TestItem>, IPackingAlgorithm> algorithmFactory,
		BenchmarkScalingScenario scenario
		)
	{
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
	#endregion
}

