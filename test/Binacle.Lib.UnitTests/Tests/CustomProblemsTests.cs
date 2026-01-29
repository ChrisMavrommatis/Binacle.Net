using Binacle.TestsKernel.Providers;

#pragma warning disable xUnit1007 


namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class CustomProblemsTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public CustomProblemsTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_FFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_FFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_WFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.WFD_v1, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_WFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.WFD_v2, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_BFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.BFD_v1, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_BFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.BFD_v2, scenario);
}
