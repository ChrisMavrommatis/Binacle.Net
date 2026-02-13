using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Results;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests.BischoffSuite;

internal class RegressionTests : ITest
{
	private readonly string title;
	private readonly string description;
	private readonly ILogger<RegressionTests> logger;
	private readonly TestAlgorithmFactory<IPackingAlgorithm>[] algorithmsUnderTest;

	public Models.ResultFile File { get; private set; }
	
	public RegressionTests(
		string title,
		string description,
		Models.ResultFile file,
		TestAlgorithmFactory<IPackingAlgorithm>[] algorithmsUnderTest,
		ILogger<RegressionTests> logger
	)
	{
		this.title = title;
		this.description = description;
		this.File = file;
		this.logger = logger;
		this.algorithmsUnderTest = algorithmsUnderTest;
	}
	
	public TestResult Run()
	{
		var scenarioCollectionResults = new ScenarioCollectionResult<double>("Scenario Name");
		foreach (var scenario in BischoffSuiteScenarioRegistry.GetScenarios())
		{
			var algorithmResults = new AlgorithmResult<double>();
			foreach (var algorithmFactory in this.algorithmsUnderTest)
			{
				var algorithmInstance = algorithmFactory(scenario!.Bin, scenario.Items);

				var result = algorithmInstance.Execute(new TestOperationParameters
				{
					Operation = AlgorithmOperation.Packing
				});

				var algorithmIdentifier = algorithmInstance.GetAlgorithmIdentifierName();

				algorithmResults.Add(algorithmIdentifier, (double)result.PackedBinVolumePercentage);
			}
			if(algorithmResults.Values.Distinct().Count() != 1)
			{
				this.logger.LogWarning(
					"Regression detected in scenario {ScenarioName}: {AlgorithmResults}",
					scenario.Name,
					string.Join(", ", algorithmResults.Select(kv => $"{kv.Key}: {kv.Value}"))
				);
				scenarioCollectionResults.Add(scenario!.Name, algorithmResults);
			}

		}

		return new TestResult()
		{
			Title = this.title,
			File = this.File,
			Description = this.description,
			Result = scenarioCollectionResults
		};
	}
}
