using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Algorithms.ExtensionMethods;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Lib.UnitTests;

public sealed class CommonTestingFixture : IDisposable
{
	public TestAlgorithmFactory<IPackingAlgorithm>[] AlgorithmsUnderTest { get; }

	public CommonTestingFixture()
	{
		this.AlgorithmsUnderTest = new TestAlgorithmFactory<IPackingAlgorithm>[]
		{
			AlgorithmFactories.FFD_v1,
			AlgorithmFactories.FFD_v2,
			AlgorithmFactories.WFD_v1,
			AlgorithmFactories.WFD_v2,
			AlgorithmFactories.BFD_v1,
			AlgorithmFactories.BFD_v2
		};
	}


	public void Dispose()
	{
	}

	// Arrange. The scenario carries both the input and what the result is measured against, so the test holds
	// onto it and hands it back at the assert.
	public Scenario GetScenarioByName(string scenarioName)
		=> AllScenariosProvider.GetScenarioByName(scenarioName);

	// Act. Just runs the algorithm - no checking, so a failure here is a broken run, not a failed expectation.
	public OperationResult Run(
		TestAlgorithmFactory<IPackingAlgorithm> algorithmFactory,
		Scenario scenario,
		AlgorithmOperation operation
	)
	{
		var algorithmInstance = algorithmFactory(scenario.Bin, scenario.Items);
		var parameters = new TestOperationParameters
		{
			Operation = operation
		};

		return algorithmInstance.Execute(parameters);
	}

	// Assert. Both checks have to run: Metrics pins how the algorithm got there, Result pins where it landed.
	[AssertionMethod]
	public void AssertResult(Scenario scenario, OperationResult result)
	{
		scenario.Metrics.EvaluateResult(result);
		scenario.Result.EvaluateResult(result);
	}
}
