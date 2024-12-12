using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.Lib.UnitTests.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Xunit;

namespace Binacle.Net.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class BinaryDecisionScenarioTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public BinaryDecisionScenarioTests(CommonTestingFixture fixture)
	{
		Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(FittingAlgorithmsBaselineScenariosProvider))]
	public void Fitting_Algorithms_Baseline_Scenarios(string algorithm, Scenario scenario)
		=> this.RunFittingScenarioTest(algorithm, scenario);

	[Theory]
	[ClassData(typeof(FittingAlgorithmsSimpleScenariosProvider))]
	public void Fitting_Algorithms_Simple_Scenarios(string algorithm, Scenario scenario)
		=> this.RunFittingScenarioTest(algorithm, scenario);

	[Theory]
	[ClassData(typeof(FittingAlgorithmsComplexScenariosProvider))]
	public void Fitting_Algorithms_Complex_Scenarios(string algorithm, Scenario scenario)
		=> this.RunFittingScenarioTest(algorithm, scenario);

	private void RunFittingScenarioTest(
		string algorithmKey,
		Scenario scenario
	)
	{
		var algorithmFactory = this.Fixture.FittingAlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new FittingParameters
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
	[ClassData(typeof(PackingAlgorithmsBaselineScenariosProvider))]
	public void Packing_Algorithms_Baseline_Scenarios(string algorithm, Scenario scenario)
		=> this.RunPackingScenarioTest(algorithm, scenario);

	[Theory]
	[ClassData(typeof(PackingAlgorithmsSimpleScenariosProvider))]
	public void Packing_Algorithms_Simple_Scenarios(string algorithm, Scenario scenario)
		=> this.RunPackingScenarioTest(algorithm, scenario);

	[Theory]
	[ClassData(typeof(PackingAlgorithmsComplexScenariosProvider))]
	public void Packing_Algorithms_Complex_Scenarios(string algorithm, Scenario scenario)
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
