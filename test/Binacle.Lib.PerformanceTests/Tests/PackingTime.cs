using System.Diagnostics;
using Binacle.Lib.Packing.Models;
using Binacle.Lib.PerformanceTests.Models;
using Binacle.Lib.PerformanceTests.Services;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Data.Providers.PackingEfficiency;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.Logging;

namespace Binacle.Lib.PerformanceTests.Tests;

internal class PackingTime : ITest
{
	private readonly OrLibraryScenarioDataProvider scenarioProvider;
	private readonly BinCollectionsDataProvider binDataProvider;
	private readonly AlgorithmFamiliesCollection algorithmFamilies;
	private readonly ILogger<PackingTime> logger;

	public PackingTime(
		OrLibraryScenarioDataProvider scenarioProvider,
		BinCollectionsDataProvider binDataProvider,
		AlgorithmFamiliesCollection algorithmFamilies,
		ILogger<PackingTime> logger
	)
	{
		this.scenarioProvider = scenarioProvider;
		this.binDataProvider = binDataProvider;
		this.algorithmFamilies = algorithmFamilies;
		this.logger = logger;
	}

	public TestResultList Run()
	{
		var times = new AlgorithmResults<double>();

		foreach (var (family, algorithms) in this.algorithmFamilies)
		{
			foreach (var objectArray in this.scenarioProvider)
			{
				var scenario = objectArray[0] as Scenario;
				var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();

				foreach (var (algorithmName, algorithmFactory) in algorithms)
				{
					var bin = scenario.GetTestBin(this.binDataProvider);

					var algorithmInstance = algorithmFactory(bin, scenario.Items);

					var time = this.MeasureAverageForRuns(10, () =>
					{
						var result = algorithmInstance.Execute(new PackingParameters
						{
							NeverReportUnpackedItems = false,
							ReportPackedItemsOnlyWhenFullyPacked = false,
							OptInToEarlyFails = true
						});
					});

					this.logger.LogDebug(
						"{AlgorithmName} {ScenarioName}: Time: {Time}",
						algorithmName,
						scenario.Name,
						time
					);

					times.AddValue(algorithmName, time.TotalMicroseconds);
				}
			}
		}

		var results = new TestResultList()
		{
			Title = "Packing Time",
			Description = "Shows the packing time of each algorithm",
			Filename = "PackingTime"
		};

		var packingTimeResults = times.GetMeasurementResults("us");
		results.Add(new TestResult()
		{
			Title = "Packing Times",
			// Description = "Shows the packing efficiency of each algorithm",
			Result = packingTimeResults
		});
		
		return results;
	}
	
	private TimeSpan MeasureAverageForRuns(int runs, Action action)
	{
		var totalElapsed = TimeSpan.Zero;
		for (int i = 0; i < runs; i++)
		{
			var startTime = Stopwatch.GetTimestamp();
			action();
			var elapsedTime = Stopwatch.GetElapsedTime(startTime);

			totalElapsed += elapsedTime;
		}
		return totalElapsed / runs;
	}
}
