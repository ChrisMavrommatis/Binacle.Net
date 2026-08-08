using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class FittingBischoffSuiteTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }

	public FittingBischoffSuiteTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_FFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.FFD_v1, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_FFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.FFD_v2, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_WFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.WFD_v1, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_WFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.WFD_v2, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_BFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.BFD_v1, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_BFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.BFD_v2, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}
}
