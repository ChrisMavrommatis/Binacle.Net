using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Packing.Models;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Services;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests;

internal class PackingEfficiencyComparison : ITest
{
	private readonly ORLibraryScenarioTestDataProvider scenarioProvider;
	private readonly BinCollectionsTestDataProvider binsProvider;
	private readonly DistinctAlgorithmCollection algorithms;
	private readonly ILogger<PackingEfficiencyComparison> logger;
	private readonly string baselineAlgorithm;

	public PackingEfficiencyComparison(
		ORLibraryScenarioTestDataProvider scenarioProvider,
		BinCollectionsTestDataProvider binsProvider,
		DistinctAlgorithmCollection algorithms,
		ILogger<PackingEfficiencyComparison> logger
	)
	{
		this.scenarioProvider = scenarioProvider;
		this.binsProvider = binsProvider;
		this.algorithms = algorithms;
		this.logger = logger;
		this.baselineAlgorithm = "BFD";
	}


	public TestResultList Run()
	{
		var discrepanciesTracker = new ScenarioDiscrepancyTracker<decimal>();

		foreach (var objectArray in this.scenarioProvider)
		{
			var scenario = (objectArray[0] as Scenario)!;

			if (!this.algorithms.TryGetValue(this.baselineAlgorithm, out var baselineAlgorithm))
			{
				throw new InvalidOperationException("Algorithm factory not found");
			}

			var baselineResult = this.RunPackingAlgorithm(baselineAlgorithm, scenario);
			discrepanciesTracker.AddValue(scenario.Name, this.baselineAlgorithm,
				baselineResult.PackedBinVolumePercentage);
			this.logger.LogDebug(
				"Baseline for {AlgorithmName} {ScenarioName}: {PackedBinVolumePercentage}",
				this.baselineAlgorithm,
				scenario!.Name,
				baselineResult.PackedBinVolumePercentage
			);

			foreach (var (algorithm, factory) in this.algorithms)
			{
				if (algorithm == this.baselineAlgorithm)
				{
					continue;
				}
				
				var result = this.RunPackingAlgorithm(factory, scenario);
				discrepanciesTracker.AddValue(scenario.Name, algorithm, result.PackedBinVolumePercentage);
				
				if (result.PackedBinVolumePercentage > baselineResult.PackedBinVolumePercentage)
				{
					this.logger.LogDebug(
						"{AlgorithmName} {ScenarioName}: {PackedBinVolumePercentage} {Symbol} {BaselinePackedBinVolumePercentage}",
						algorithm,
						scenario.Name,
						result.PackedBinVolumePercentage,
						">",
						baselineResult.PackedBinVolumePercentage
					);
				}
			}
		}

		var results = new TestResultList()
		{
			Title = "Packing Efficiency Comparison",
			Description = "Shows the packing efficiency of each algorithm",
			Filename = "PackingEfficiencyComparison"
		};

		var comparisonResults = discrepanciesTracker.GetDiscrepanciesComparisonResults("%", this.baselineAlgorithm);
		results.Add(new TestResult()
		{
			Title = "Packing Efficiency Comparison",
			// Description = "",
			Result = comparisonResults
		});


		return results;
	}

	private PackingResult RunPackingAlgorithm<TAlgorithm>(
		AlgorithmFactory<TAlgorithm> algorithmFactory,
		Scenario scenario
	)
		where TAlgorithm : class, IPackingAlgorithm
	{
		var bin = scenario.GetTestBin(this.binsProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new PackingParameters
		{
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false,
			OptInToEarlyFails = true
		});

		return result;
	}
}
