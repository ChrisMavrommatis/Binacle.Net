using Binacle.Net.Lib.PerformanceTests.Models;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal class PackingEfficiencyPerformanceTest : ITest
{
	private readonly ORLibraryScenarioTestDataProvider scenarioProvider;
	private readonly BinCollectionsTestDataProvider binTestDataProvider;
	private readonly ILogger<PackingEfficiencyPerformanceTest> logger;
	private readonly AlgorithmFamiliesCollection algorithmFamilyFactories;

	public PackingEfficiencyPerformanceTest(
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
	}

	public List<TestSummaryAction> Run()
	{
		var timeResults = new AlgorithmsTestResults<TimeSpan>();

		foreach (var (algorithmFamilyKey, algorithmFactories) in this.algorithmFamilyFactories)
		{
			foreach (var objects in this.scenarioProvider)
			{
				var scenario = objects[0] as Scenario;
				var scenarioResult = scenario!.ResultAs<PackingEfficiencyScenarioResult>();
				foreach (var (algorithmName, algorithmFactory) in algorithmFactories)
				{
					var bin = scenario.GetTestBin(this.binTestDataProvider);

					var algorithmInstance = algorithmFactory(bin, scenario.Items);

					var time = this.MeasureAverageForRuns(1, () =>
					{
						var result = algorithmInstance.Execute(new Packing.Models.PackingParameters
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
					timeResults.AddValue(algorithmName, time);
				}
			}
		}
		var results = new List<TestSummaryAction>();
		results.Add(new TestSummaryAction
		{
			Name = "Packing Efficiency Performance Results",
			Action = () =>
			{
				foreach (var (algorithmName, times) in timeResults)
				{
					var min = Math.Round(times.Min(x => x.TotalMicroseconds), 4);
					var avg = Math.Round(times.Average(x => x.TotalMicroseconds), 4);
					var max = Math.Round(times.Max(x => x.TotalMicroseconds), 4);
					this.logger.LogInformation(
						"Algorithm: {AlgorithmName}. Time(us) Min: {Min}, Avg: {Avg}, Max: {Max}",
						algorithmName, min, avg, max
					);
				}
			}
		});

		return results;
	}

	internal TimeSpan MeasureAverageForRuns(int runs, Action action)
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

