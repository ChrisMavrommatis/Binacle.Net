using Binacle.Lib.Abstractions.Algorithms;
using Binacle.TestsKernel;
using Binacle.TestsKernel.ExtensionMethods;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;

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

	public void RunTest(
		TestAlgorithmFactory<IPackingAlgorithm> algorithmFactory,
		string scenarioName
	)
	{
		var scenario = AllScenariosRegistry.GetScenarioByName(scenarioName);
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
