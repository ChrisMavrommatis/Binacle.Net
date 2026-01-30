using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Results;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests.BischoffSuite;

internal class BaselineComparisonTests : ITest
{
	private readonly string title;
	private readonly string description;
	private readonly string filename;
	private readonly TestAlgorithmFactory<IPackingAlgorithm> baselineAlgorithm;
	private readonly TestAlgorithmFactory<IPackingAlgorithm>[] algorithmsUnderTest;
	private readonly ILogger<BaselineComparisonTests> logger;

	public BaselineComparisonTests(
		string title,
		string description,
		string filename,
		TestAlgorithmFactory<IPackingAlgorithm> baselineAlgorithm,
		TestAlgorithmFactory<IPackingAlgorithm>[] algorithmsUnderTest,
		ILogger<BaselineComparisonTests> logger
		
	)
	{
		this.title = title;
		this.description = description;
		this.filename = filename;
		this.baselineAlgorithm = baselineAlgorithm;
		this.algorithmsUnderTest = algorithmsUnderTest;
		this.logger = logger;
	}

	public TestResult Run()
	{
		var scenarioCollectionResults = new ScenarioCollectionResult<double>("Scenario Name");
		foreach (var scenario in BischoffSuiteScenarioRegistry.GetScenarios())
		{
			var algorithmResults = new AlgorithmResult<double>();
			var baselineAlgorithmInstance = this.baselineAlgorithm(scenario!.Bin, scenario.Items);
			
			var baselineResult = baselineAlgorithmInstance.Execute(new TestOperationParameters
			{
				Operation = AlgorithmOperation.Packing
			});
			var baselineAlgorithmIdentifier = baselineAlgorithmInstance.GetAlgorithmIdentifierName();
			algorithmResults.Add(baselineAlgorithmIdentifier, (double)baselineResult.PackedBinVolumePercentage);

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
			
			// find if any result is better than the baseline
			foreach (var kvp in algorithmResults)
			{
				if (kvp.Key == baselineAlgorithmIdentifier)
					continue;
				if (kvp.Value > algorithmResults[baselineAlgorithmIdentifier])
				{
					this.logger.LogWarning("Algorithm {Algorithm} performed better than baseline {Baseline} on scenario {Scenario}: {Value} > {BaselineValue}",
						kvp.Key,
						baselineAlgorithmIdentifier,
						scenario.Name,
						kvp.Value,
						algorithmResults[baselineAlgorithmIdentifier]
					);
					scenarioCollectionResults.Add(scenario!.Name, algorithmResults);
					break;
				}
			}
		}

		return new TestResult()
		{
			Title = this.title,
			Description = this.description,
			Filename = this.filename,
			Result = scenarioCollectionResults
		};
	}
}
