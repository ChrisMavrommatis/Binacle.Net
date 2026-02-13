using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Results;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests.BischoffSuite;

internal class EfficiencyStatisticsTests : ITest
{
	private readonly string title;
	private readonly string description;
	private readonly ScenarioCollectionsProvider scenarioCollectionsProvider;
	private readonly TestAlgorithmFactory<IPackingAlgorithm> algorithmUnderTest;
	private readonly ILogger<EfficiencyStatisticsTests> logger;
	private readonly string[] collectionKeys;

	public Models.ResultFile File { get; private set; }
	
	public EfficiencyStatisticsTests(
		string title,
		string description,
		Models.ResultFile file,
		ScenarioCollectionsProvider scenarioCollectionsProvider,
		TestAlgorithmFactory<IPackingAlgorithm> algorithmUnderTest,
		ILogger<EfficiencyStatisticsTests> logger
	)
	{
		this.title = title;
		this.description = description;
		this.File = file;
		this.scenarioCollectionsProvider = scenarioCollectionsProvider;
		this.algorithmUnderTest = algorithmUnderTest;
		this.logger = logger;
		this.collectionKeys = CollectionKeys.BischoffSuite.ToArray();
	}

	public TestResult Run()
	{
		var collectionResults = new CollectionStatisticsResult<double>("Collection Key", "%");
		foreach(var collectionKey in this.collectionKeys)
		{
			var scenarioCollection = this.scenarioCollectionsProvider.GetScenarios(collectionKey);

			foreach (var scenario in scenarioCollection)
			{
				var algorithmInstance = this.algorithmUnderTest(scenario.Bin, scenario.Items);

				var result = algorithmInstance.Execute(new TestOperationParameters
				{
					Operation = AlgorithmOperation.Packing
				});
				collectionResults.AddValue(collectionKey,  (double)result.PackedBinVolumePercentage);
			}
		}
		
		return new TestResult()
		{
			Title = this.title,
			File = this.File,
			Description = this.description,
			Result = collectionResults
		};
	}
}
