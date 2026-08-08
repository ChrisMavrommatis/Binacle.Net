using Binacle.TestsKernel.Algorithms.Providers;

#pragma warning disable xUnit1007 


namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class PackingCustomProblemsTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public PackingCustomProblemsTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public void CustomProblems_Packing_FFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.FFD_v1, testScenario, AlgorithmOperation.Packing);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public void CustomProblems_Packing_FFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.FFD_v2, testScenario, AlgorithmOperation.Packing);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public void CustomProblems_Packing_WFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.WFD_v1, testScenario, AlgorithmOperation.Packing);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public void CustomProblems_Packing_WFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.WFD_v2, testScenario, AlgorithmOperation.Packing);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public void CustomProblems_Packing_BFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.BFD_v1, testScenario, AlgorithmOperation.Packing);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public void CustomProblems_Packing_BFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.BFD_v2, testScenario, AlgorithmOperation.Packing);

		this.Fixture.AssertResult(testScenario, result);
	}
}
