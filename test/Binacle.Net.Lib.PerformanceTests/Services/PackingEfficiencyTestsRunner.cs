using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.Lib.PerformanceTests.Models;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal class PackingEfficiencyTestsRunner : ITestsRunner
{
	private readonly ORLibraryScenarioTestDataProvider scenarioProvider;
	private readonly BinCollectionsTestDataProvider binTestDataProvider;
	private readonly ILogger<PackingEfficiencyTestsRunner> logger;
	private readonly AlgorithmFamiliesCollection algorithmFamilyFactories;

	public PackingEfficiencyTestsRunner(
		ORLibraryScenarioTestDataProvider scenarioProvider,
		BinCollectionsTestDataProvider binTestDataProvider,
		AlgorithmFamiliesCollection algorithmFamilyFactories,
		ILogger<PackingEfficiencyTestsRunner> logger
	)
	{
		this.scenarioProvider = scenarioProvider;
		this.binTestDataProvider = binTestDataProvider;
		this.algorithmFamilyFactories = algorithmFamilyFactories;
		this.logger = logger;
	}

	public List<TestSummaryAction> Run()
	{
		this.logger.LogInformation("Running Tests ...");

		var percentagesByAlgorithm = new AlgorithmsTestResults<decimal>();
		var adjustedPercentagesByAlgorithm = new AlgorithmsTestResults<decimal>();
		var scenarioFamilyDiscrepancies = new AlgorithmsDiscrepancies<decimal>();

		foreach (var (algorithmFamilyKey, algorithmFactories) in this.algorithmFamilyFactories)
		{
			foreach (var objects in this.scenarioProvider)
			{
				var scenario = objects[0] as Scenario;
				var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();
				var scenarioPercentageByAlgorithm = new Dictionary<string, decimal>();
				foreach (var (algorithmName, algorithmFactory) in algorithmFactories)
				{
					var result = this.RunPackingAlgorithm(algorithmFactory, scenario);
					scenarioPercentageByAlgorithm.Add(algorithmName, result.PackedBinVolumePercentage);

					percentagesByAlgorithm.AddValue(algorithmName, result.PackedBinVolumePercentage);
					var adjustedEfficiency = Math.Round((result.PackedBinVolumePercentage / scenarioResult.MaxPotentialPackingEfficiencyPercentage) * 100, 2);
					adjustedPercentagesByAlgorithm.AddValue(algorithmName, adjustedEfficiency);

					this.logger.LogDebug(
						"{AlgorithmName} {ScenarioName}: {PackedBinVolumePercentage}/{MaxPotentialPackingEfficiencyPercentage} = {AdjustedEfficiency}",
						algorithmName,
						scenario.Name,
						result.PackedBinVolumePercentage,
						scenarioResult.MaxPotentialPackingEfficiencyPercentage,
						adjustedEfficiency
					);
				}

				if (scenarioPercentageByAlgorithm.Values.Distinct().Count() != 1)
				{
					scenarioFamilyDiscrepancies.AddDiscrepancy(algorithmFamilyKey, scenario.Name, scenarioPercentageByAlgorithm);
				}

			}
		}


		this.logger.LogInformation("Calculating Results .... ");
		var results = new List<TestSummaryAction>();

		results.Add(new TestSummaryAction
		{
			Name = "Packing Efficiency Results",
			Action = () => 
			{
				foreach (var (algorithmName, algorithmPercentages) in percentagesByAlgorithm)
				{
					var min = algorithmPercentages.Min();
					var avg = Math.Round(algorithmPercentages.Average(), 2);
					var max = algorithmPercentages.Max();
					this.logger.LogInformation(
						"Algorithm: {AlgorithmName}. Efficiency Min: {Min}, Avg: {Avg}, Max: {Max}",
						algorithmName, min, avg, max
					);
				}
			}
		});

		results.Add(new TestSummaryAction
		{
			Name = "Packing Efficiency Adjusted Results",
			Action = () =>
			{
				foreach (var (algorithmName, algorithmPercentages) in adjustedPercentagesByAlgorithm)
				{
					var min = algorithmPercentages.Min();
					var avg = Math.Round(algorithmPercentages.Average(), 2);
					var max = algorithmPercentages.Max();
					this.logger.LogInformation(
						"Algorithm: {AlgorithmName}. Adjusted Efficiency: Min: {Min}, Avg: {Avg}, Max: {Max}",
						algorithmName, min, avg, max
					);
				}
			}
		});

		

		if (scenarioFamilyDiscrepancies.HasDiscrepancies())
		{
			results.Add(new TestSummaryAction
			{
				Name = "Packing Efficiency Discrepancy Results",
				Action = () =>
				{
					this.logger.LogWarning(
						"Warning: {NoOfDiscrepancies} discrepancies found during tests across {NoOfFamilies}", 
						scenarioFamilyDiscrepancies.TotalDiscrepancies(),
						scenarioFamilyDiscrepancies.DiscrepancyCategoriesCount()
					);
					foreach (var (familyName, discrepancies) in scenarioFamilyDiscrepancies)
					{
						this.logger.LogWarning("Family: {FamilyName}", familyName);
						foreach (var (scenarioName, scenarioDiscrepancies) in discrepancies)
						{
							this.logger.LogWarning("Scenario: {ScenarioName}", scenarioName);
							foreach (var (algorithmName, percentage) in scenarioDiscrepancies)
							{
								this.logger.LogWarning("Algorithm: {AlgorithmName}, Efficiency: {Percentage}", algorithmName, percentage);
							}
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

