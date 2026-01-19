using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.UnitTests.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;

#pragma warning disable xUnit1007 


namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class BinaryDecisionScenarioTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public BinaryDecisionScenarioTests(CommonTestingFixture fixture)
	{
		Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(AlgorithmsBaselineScenariosProvider))]
	public void Fitting_Algorithms_Baseline_Scenarios(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[ClassData(typeof(AlgorithmsSimpleScenariosProvider))]
	public void Fitting_Algorithms_Simple_Scenarios(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[ClassData(typeof(AlgorithmsComplexScenariosProvider))]
	public void Fitting_Algorithms_Complex_Scenarios(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[ClassData(typeof(AlgorithmsBaselineScenariosProvider))]
	public void Packing_Algorithms_Baseline_Scenarios(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(AlgorithmsSimpleScenariosProvider))]
	public void Packing_Algorithms_Simple_Scenarios(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(AlgorithmsComplexScenariosProvider))]
	public void Packing_Algorithms_Complex_Scenarios(string algorithm, Scenario scenario)
		=> this.RunScenarioTest(algorithm, scenario, AlgorithmOperation.Packing);

	private void RunScenarioTest(
		string algorithmKey,
		Scenario scenario,
		AlgorithmOperation operation
	)
	{
		var algorithmFactory = this.Fixture.AlgorithmsUnderTest[algorithmKey];
		var bin = scenario.GetTestBin(this.Fixture.BinDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new OperationParameters
		{
			Operation = operation
		});

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			result.Status.ShouldBe(OperationResultStatus.FullyPacked);
		}
		else
		{
			result.Status.ShouldNotBe(OperationResultStatus.FullyPacked);
		}
	}
}
