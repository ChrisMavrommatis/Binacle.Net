using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Services;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Data.Providers.PackingEfficiency;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.Logging;


namespace Binacle.Lib.PerformanceTests.Tests;


internal class BischoffSuitePackingEfficiency : ITest
{
	private readonly BischoffSuiteDataProvider scenarioProvider;

	public BischoffSuitePackingEfficiency(
		BischoffSuiteDataProvider scenarioProvider
		)
	{
		this.scenarioProvider = scenarioProvider;
	}
	public TestResultList Run()
	{
		foreach (var objectArray in this.scenarioProvider)
		{
			var scenario = objectArray[0] as Scenario;
			var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();

			foreach (var algorithmFactory in AlgorithmFactories.All)
			{
					var result = this.RunAlgorithm(algorithmFactory, scenario);
			}
		}
		
		var results = new TestResultList()
		{
			Title = "Bischoff Suite Packing Efficiency",
			Description = "Shows the packing efficiency of each algorithm in the Bischoff Suite",
			Filename = "BischoffSuitePackingEfficiency"
		};
		return results;
	}
}

internal class PackingEfficiency : ITest
{
	private readonly AlgorithmFamiliesCollection algorithmFamilies;
	private readonly BischoffSuiteDataProvider scenarioProvider;
	private readonly BinCollectionsDataProvider binsProvider;
	private readonly ILogger<PackingEfficiency> logger;

	public PackingEfficiency(
		AlgorithmFamiliesCollection algorithmFamilies,
		BischoffSuiteDataProvider scenarioProvider,
		BinCollectionsDataProvider binsProvider,
		ILogger<PackingEfficiency> logger
		)
	{
		this.algorithmFamilies = algorithmFamilies;
		this.scenarioProvider = scenarioProvider;
		this.binsProvider = binsProvider;
		this.logger = logger;
	}
	public TestResultList Run()
	{
		var packingPercentages = new AlgorithmResults<double>();
		var adjustedPackingPercentages = new AlgorithmResults<double>();
		var discrepancies = new AlgorithmDiscrepancyTracker<double>();
			
		foreach (var (family, algorithms) in this.algorithmFamilies)
		{
			foreach (var objectArray in this.scenarioProvider)
			{
				var scenario = objectArray[0] as Scenario;
				var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();
				var scenarioDiscrepancyTracker = new Dictionary<string, double>();
				
				foreach(var (algorithmName, algorithmFactory) in algorithms)
				{
					var result = this.RunAlgorithm(algorithmFactory, scenario);
					scenarioDiscrepancyTracker.Add(algorithmName, (double)result.PackedBinVolumePercentage);
					packingPercentages.AddValue(algorithmName, (double)result.PackedBinVolumePercentage);
					var adjustedPackingPercentage =  Math.Round((result.PackedBinVolumePercentage / scenarioResult.MaxPotentialPackingEfficiencyPercentage) * 100, 2);
					adjustedPackingPercentages.AddValue(algorithmName, (double)adjustedPackingPercentage);
					
					this.logger.LogDebug(
						"{AlgorithmName} {ScenarioName}: {PackedBinVolumePercentage}/{MaxPotentialPackingEfficiencyPercentage} = {AdjustedEfficiency}",
						algorithmName,
						scenario.Name,
						result.PackedBinVolumePercentage,
						scenarioResult.MaxPotentialPackingEfficiencyPercentage,
						adjustedPackingPercentage
					);
				}
				
				if (scenarioDiscrepancyTracker.Values.Distinct().Count() != 1)
				{
					discrepancies.AddDiscrepancy(family, scenario.Name, scenarioDiscrepancyTracker);
				}
			}
		}
		
		var results = new TestResultList()
		{
			Title = "Packing Efficiency",
			Description = "Shows the packing efficiency of each algorithm",
			Filename = "PackingEfficiency"
		};

		var packingEfficiencyResults = packingPercentages.GetMeasurementResults("%");
		results.Add(new TestResult()
		{
			Title = "Packing Efficiency Results",
			// Description = "Shows the packing efficiency of each algorithm",
			Result = packingEfficiencyResults
		});
		
		var adjustedPackingEfficiencyResults = adjustedPackingPercentages.GetMeasurementResults("%");
		results.Add(new TestResult()
		{
			Title = "Packing Efficiency Adjusted Results",
			// Description = "Shows the adjusted packing efficiency of each algorithm",
			Result = adjustedPackingEfficiencyResults
		});
		
		if (discrepancies.HasDiscrepancies())
		{
			var discrepancyResults = discrepancies.GetDiscrepancyResults("%");
			foreach(var discrepancyResult in discrepancyResults)
			{
				results.Add(new TestResult()
				{
					Title = $"{discrepancyResult.Family} Packing Efficiency Discrepancies",
					// Description = "Shows the discrepancies in packing efficiency between algorithms",
					Result = discrepancyResult
				});
			}
		}

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
