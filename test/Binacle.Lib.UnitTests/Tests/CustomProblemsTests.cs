using Binacle.Lib.Abstractions.Algorithms;
using Binacle.TestsKernel;
using Binacle.TestsKernel.ExtensionMethods;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;

#pragma warning disable xUnit1007 


namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class CustomProblemsTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public CustomProblemsTests(CommonTestingFixture fixture)
	{
		Fixture = fixture;
	}

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_FFD_v1(string scenario)
		=> this.Run(AlgorithmFactories.FFD_v1, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_FFD_v2(string scenario)
		=> this.Run(AlgorithmFactories.FFD_v2, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_WFD_v1(string scenario)
		=> this.Run(AlgorithmFactories.WFD_v1, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_WFD_v2(string scenario)
		=> this.Run(AlgorithmFactories.WFD_v2, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_BFD_v1(string scenario)
		=> this.Run(AlgorithmFactories.BFD_v1, scenario);

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public void CustomProblems_BFD_v2(string scenario)
		=> this.Run(AlgorithmFactories.BFD_v2, scenario);


	private void Run(
		TestAlgorithmFactory<IPackingAlgorithm> algorithmFactory,
		string scenarioName
	)
	{
		var scenario = CustomProblemsScenarioProvider.GetScenarioByName(scenarioName);
		var algorithmInstance = algorithmFactory(scenario.Bin, scenario.Items);

		var result = algorithmInstance.Execute(new TestOperationParameters
		{
			Operation = AlgorithmOperation.Packing
		});
		scenario.Metrics.EvaluateResult(result);
		
		// if (scenarioResult.Fits)
		// {
		// 	result.Status.ShouldBe(OperationResultStatus.FullyPacked);
		// }
		// else
		// {
		// 	result.Status.ShouldNotBe(OperationResultStatus.FullyPacked);
		// }
	}

}
