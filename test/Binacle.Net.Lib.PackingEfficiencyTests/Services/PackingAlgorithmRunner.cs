using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.Lib.PackingEfficiencyTests.Data.Providers.PackingEfficiency;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.Logging;

namespace Binacle.Net.Lib.PackingEfficiencyTests.Services;


internal interface ITestsRunner
{
	void Run();
}

internal class PackingEfficiencyTestsRunner : ITestsRunner
{
	private readonly ORLibraryScenarioTestDataProvider scenarioProvider;
	private readonly BinCollectionsTestDataProvider binTestDataProvider;
	private readonly ILogger<PackingEfficiencyTestsRunner> logger;
	private readonly Dictionary<string, Func<TestBin, List<TestItem>, IPackingAlgorithm>> algorithmFactories;

	public PackingEfficiencyTestsRunner(
		ORLibraryScenarioTestDataProvider scenarioProvider,
		BinCollectionsTestDataProvider binTestDataProvider,
		ILogger<PackingEfficiencyTestsRunner> logger
	)
	{
		this.scenarioProvider = scenarioProvider;
		this.binTestDataProvider = binTestDataProvider;
		this.logger = logger;
		this.algorithmFactories = new()
		{
			{ "Packing_FFD_v1", AlgorithmFactories.Packing_FFD_v1 },
			{ "Packing_FFD_v2", AlgorithmFactories.Packing_FFD_v2 },
			{ "Packing_FFD_v3", AlgorithmFactories.Packing_FFD_v3 },
			{ "Packing_FFD_v4", AlgorithmFactories.Packing_FFD_v4 }
		};

	}

	public void Run()
	{
		this.logger.LogInformation("Running Tests ...");

		var percentagesByAlgorithm = new Dictionary<string, List<decimal>>();
		var scenarioDiscrepancies = new Dictionary<string, Dictionary<string, decimal>>();
		foreach (var objects in this.scenarioProvider)
		{
			var scenario = objects[0] as Scenario;
			var scenarioResult = scenario.ResultAs<PackingEfficiencyScenarioResult>();

			var scenarioPercentageByAlgorithm = new Dictionary<string, decimal>();
			foreach (var (algorithmName, algorithmFactory) in this.algorithmFactories)
			{
				var result = this.RunPackingAlgorithm(algorithmFactory, scenario);

				scenarioPercentageByAlgorithm.Add(algorithmName, result.PackedBinVolumePercentage);

				if (percentagesByAlgorithm.TryGetValue(algorithmName, out var algorithmPercentages))
				{
					algorithmPercentages.Add(result.PackedBinVolumePercentage);
				}
				else
				{
					algorithmPercentages = new List<decimal> { result.PackedBinVolumePercentage };
					percentagesByAlgorithm.Add(algorithmName, algorithmPercentages);
				}


				var adjustedEfficiency = Math.Round((result.PackedBinVolumePercentage / scenarioResult.MaxPotentialPackingEfficiencyPercentage) * 100, 2);

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
				var copy = new Dictionary<string, decimal>(scenarioPercentageByAlgorithm);
				scenarioDiscrepancies.Add(scenario.Name, copy);
			}
		}

		this.logger.LogInformation("Calculating Results .... ");

		foreach (var (algorithmName, algorithmPercentages) in percentagesByAlgorithm)
		{
			var averagePackedBinVolumePercentage = algorithmPercentages.Average();
			this.logger.LogInformation("Algorithm: {AlgorithmName}, Average Efficiency: {AveragePackedBinVolumePercentage}", algorithmName, averagePackedBinVolumePercentage);
		}

		if (scenarioDiscrepancies.Count > 0)
		{
			this.logger.LogWarning("Warning: {NoOfDiscrepancies} discrepancies found during tests", scenarioDiscrepancies.Count);
			foreach (var (scenarioName, scenarioPercentageByAlgorithm) in scenarioDiscrepancies)
			{
				this.logger.LogWarning("Discrepancy in Scenario: {ScenarioName}", scenarioName);
				foreach (var (algorithmName, percentage) in scenarioPercentageByAlgorithm)
				{
					this.logger.LogWarning("Algorithm: {AlgorithmName}, Efficiency: {Percentage}", algorithmName, percentage);
				}
			}

		}
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

