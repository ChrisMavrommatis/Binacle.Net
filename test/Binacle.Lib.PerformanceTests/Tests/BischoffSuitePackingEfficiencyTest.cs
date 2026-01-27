using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Services;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests;

internal class BischoffSuitePackingEfficiencyTest : ITest
{
	private readonly BischoffSuiteDataProvider dataProvider;
	private readonly ILogger<BischoffSuitePackingEfficiencyTest> logger;
	private readonly TestAlgorithmFactory<IPackingAlgorithm>[] algorithmsUnderTest;

	public BischoffSuitePackingEfficiencyTest(
		BischoffSuiteDataProvider dataProvider,
		ILogger<BischoffSuitePackingEfficiencyTest> logger
	)
	{
		this.dataProvider = dataProvider;
		this.logger = logger;
		this.algorithmsUnderTest =
		[
			AlgorithmFactories.FFD_v1,
			AlgorithmFactories.FFD_v2,
			AlgorithmFactories.WFD_v1,
			AlgorithmFactories.WFD_v2,
			AlgorithmFactories.BFD_v1,
			AlgorithmFactories.BFD_v2
		];
	}

public TestResult Run()
{
	var scenarioCollectionResults = new ScenarioCollectionResult<double>("Scenario Name");
	foreach (var objectArray in this.dataProvider)
	{
		var scenario = objectArray[0] as Scenario;
		// var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();

		var algorithmResults = new ColumnResult<double>();
		foreach (var algorithmFactory in this.algorithmsUnderTest)
		{
			var algorithmInstance = algorithmFactory(scenario!.Bin, scenario.Items);

			var result = algorithmInstance.Execute(new TestOperationParameters
			{
				Operation = AlgorithmOperation.Packing
			});

			var algorithmIdentifier = algorithmInstance.GetAlgorithmIdentifierName();

			algorithmResults.Add(algorithmIdentifier, (double)result.PackedBinVolumePercentage);

			this.logger.LogDebug(
				"{ScenarioName} - {AlgorithmIdentifierName}: {PackedBinVolumePercentage}",
				scenario.Name,
				algorithmIdentifier,
				result.PackedBinVolumePercentage
			);
		}

		scenarioCollectionResults.Add(scenario!.Name, algorithmResults);
	}

	return new TestResult()
	{
		Title = "Bischoff Suite Packing Efficiency Test",
		Description = "Packing Efficiency test using the Bischoff Suite scenarios.",
		Filename = "BischoffSuitePackingEfficiency",
		Result = scenarioCollectionResults
	};
}

}
