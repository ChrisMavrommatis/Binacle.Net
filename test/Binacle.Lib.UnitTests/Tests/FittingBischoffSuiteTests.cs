using Binacle.TestsKernel.ScenarioProviders;

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
		=> this.Fixture.RunTest(AlgorithmFactories.FFD_v1, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_FFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.FFD_v2, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_WFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.WFD_v1, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_WFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.WFD_v2, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_BFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.BFD_v1, scenario, AlgorithmOperation.Fitting);

	[Theory]
	[MemberData(nameof(BischoffSuiteScenarioProvider.ScenarioNames), MemberType = typeof(BischoffSuiteScenarioProvider))]
	public void OR_Library_Fitting_BFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.BFD_v2, scenario, AlgorithmOperation.Fitting);
}
