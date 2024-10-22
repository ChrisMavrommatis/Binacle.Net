using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.Lib.PerformanceTests.Models;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal class PackingEfficiencyBaselineComparisonTest : ITest
{
	private readonly ORLibraryScenarioTestDataProvider scenarioProvider;
	private readonly BinCollectionsTestDataProvider binTestDataProvider;
	private readonly ILogger<PackingEfficiencyPerformanceTest> logger;
	private readonly string baselineAlgorithmFamilyKey;
	private readonly string baselineAlgorithmName;
	private readonly AlgorithmFamiliesCollection algorithmFamilyFactories;

	public PackingEfficiencyBaselineComparisonTest(
		ORLibraryScenarioTestDataProvider scenarioProvider,
		BinCollectionsTestDataProvider binTestDataProvider,
		AlgorithmFamiliesCollection algorithmFamilyFactories,
		ILogger<PackingEfficiencyPerformanceTest> logger
	)
	{
		this.scenarioProvider = scenarioProvider;
		this.binTestDataProvider = binTestDataProvider;
		this.algorithmFamilyFactories = algorithmFamilyFactories;
		this.logger = logger;
		this.baselineAlgorithmFamilyKey = "BFD";
		this.baselineAlgorithmName = "Packing_BFD_v1";
	}

	public List<TestSummaryAction> Run()
	{
		if (!this.algorithmFamilyFactories.TryGetValue(this.baselineAlgorithmFamilyKey, out var baselineFamilyAlgorithmFactories))
		{
			throw new InvalidOperationException("Algorithm family not found");
		}

		if (!baselineFamilyAlgorithmFactories.TryGetValue(this.baselineAlgorithmName, out var baselineAlgorithmFactory))
		{
			throw new InvalidOperationException("Algorithm factory not found");
		}

		var baselineAlgorithmScenarioResults = new Dictionary<string, decimal>();

		foreach (var objects in this.scenarioProvider)
		{
			var scenario = objects[0] as Scenario;
			var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();
			var result = this.RunPackingAlgorithm(baselineAlgorithmFactory, scenario);
			this.logger.LogDebug(
				"Baseline for {AlgorithmName} {ScenarioName}: {PackedBinVolumePercentage}",
				this.baselineAlgorithmName,
				scenario.Name,
				result.PackedBinVolumePercentage
			);
			baselineAlgorithmScenarioResults.Add(scenario.Name, result.PackedBinVolumePercentage);
		}

		var discrepancies = new Dictionary<string, Dictionary<string, decimal>>();
		foreach (var (algorithmFamilyKey, algorithmFactories) in this.algorithmFamilyFactories)
		{
			foreach (var objects in this.scenarioProvider)
			{
				var scenario = objects[0] as Scenario;
				var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();
				foreach (var (algorithmName, algorithmFactory) in algorithmFactories)
				{
					if (algorithmName == this.baselineAlgorithmName)
					{
						continue;
					}

					var result = this.RunPackingAlgorithm(algorithmFactory, scenario);
					var baselineAlgorithmResult = baselineAlgorithmScenarioResults[scenario.Name];

					var resultIsEqualToBaseline = result.PackedBinVolumePercentage == baselineAlgorithmResult;
					var resultIsGreaterThanBaseline = result.PackedBinVolumePercentage > baselineAlgorithmResult;
					var isDiscrepancy = resultIsEqualToBaseline || resultIsGreaterThanBaseline;
					if (isDiscrepancy)
					{
						if(discrepancies.TryGetValue(scenario.Name, out var discrepancy))
						{
							discrepancy.Add(algorithmName, result.PackedBinVolumePercentage);
						}
						else
						{
							discrepancies.Add(scenario.Name, new Dictionary<string, decimal> { { algorithmName, result.PackedBinVolumePercentage } });
						}	
					}
					var symbol = !isDiscrepancy ? "<" : (resultIsEqualToBaseline ? "=" : ">");
					this.logger.LogDebug(
						"{AlgorithmName} {ScenarioName}: {PackedBinVolumePercentage} {Symbol} {BaselinePackedBinVolumePercentage}",
						algorithmName,
						scenario.Name,
						result.PackedBinVolumePercentage,
						symbol,
						baselineAlgorithmResult
					);
				}
			}
		}

		var results = new List<TestSummaryAction>();

		if (discrepancies.Count > 0)
		{
			results.Add(new TestSummaryAction
			{
				Name = "Packing Efficiency Baseline Comparison Results",
				Action = () =>
				{
					foreach (var (scenarioName, discrepancy) in discrepancies)
					{
						this.logger.LogWarning("Scenario: {ScenarioName}", scenarioName);
						var baselinePackedBinVolumePercentage = baselineAlgorithmScenarioResults[scenarioName];
						foreach (var (algorithmName, packedBinVolumePercentage) in discrepancy)
						{
							var symbol = packedBinVolumePercentage > baselinePackedBinVolumePercentage ? ">" : "=";
							this.logger.LogWarning(
								"{AlgorithmName}: {PackedBinVolumePercentage} {Symbol} {BaselinePackedBinVolumePercentage}", 
								algorithmName, 
								packedBinVolumePercentage,
								symbol,
								baselinePackedBinVolumePercentage
							);
						}
					}
				}
			});
		}




		return results;
	}

	internal PackingResult RunPackingAlgorithm<TAlgorithm>(
			Func<TestBin, List<TestItem>, TAlgorithm> algorithmFactory,
			Scenario scenario
	)
		where TAlgorithm : class, IPackingAlgorithm
	{
		var bin = scenario.GetTestBin(this.binTestDataProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new Packing.Models.PackingParameters
		{
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false,
			OptInToEarlyFails = true
		});

		return result;

	}


}

