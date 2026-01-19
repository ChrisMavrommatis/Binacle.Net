using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Services;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Data.Providers.PackingEfficiency;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests;

internal class PackingEfficiencyComparison : ITest
{
	private readonly OrLibraryScenarioDataProvider scenarioProvider;
	private readonly BinCollectionsDataProvider binsProvider;
	private readonly DistinctAlgorithmCollection algorithms;
	private readonly ILogger<PackingEfficiencyComparison> logger;
	private readonly string baselineAlgorithm;

	public PackingEfficiencyComparison(
		OrLibraryScenarioDataProvider scenarioProvider,
		BinCollectionsDataProvider binsProvider,
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

			var baselineResult = this.RunAlgorithm(baselineAlgorithm, scenario);
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
				
				var result = this.RunAlgorithm(factory, scenario);
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

	private OperationResult RunAlgorithm<TAlgorithm>(
		AlgorithmFactory<TAlgorithm> algorithmFactory,
		Scenario scenario
	)
		where TAlgorithm : class, IPackingAlgorithm
	{
		var bin = scenario.GetTestBin(this.binsProvider);

		var algorithmInstance = algorithmFactory(bin, scenario.Items);

		var result = algorithmInstance.Execute(new OperationParameters()
		{
			Operation = AlgorithmOperation.Packing
		});

		return result;
	}
}
