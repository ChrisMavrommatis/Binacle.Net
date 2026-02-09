using Binacle.TestsKernel.Providers;

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
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_Packing_FFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.FFD_v1, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_Packing_FFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.FFD_v2, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_Packing_WFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.WFD_v1, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_Packing_WFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.WFD_v2, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_Packing_BFD_v1(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.BFD_v1, scenario, AlgorithmOperation.Packing);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_Packing_BFD_v2(string scenario)
		=> this.Fixture.RunTest(AlgorithmFactories.BFD_v2, scenario, AlgorithmOperation.Packing);
}
