using System.Numerics;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Packing.Models;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Services;

internal class PackingEfficiency : ITest
{
	private readonly AlgorithmFamiliesCollection algorithmFamilies;
	private readonly ORLibraryScenarioTestDataProvider scenarioProvider;
	private readonly BinCollectionsTestDataProvider binsProvider;
	private readonly ILogger<PackingEfficiency> logger;

	public PackingEfficiency(
		AlgorithmFamiliesCollection algorithmFamilies,
		ORLibraryScenarioTestDataProvider scenarioProvider,
		BinCollectionsTestDataProvider binsProvider,
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
					var result = this.RunPackingAlgorithm(algorithmFactory, scenario);
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
			// Need to do this
		}

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
