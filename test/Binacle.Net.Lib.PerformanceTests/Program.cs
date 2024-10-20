using Binacle.Net.Lib.PerformanceTests.Models;
using Binacle.Net.Lib.PerformanceTests.Services;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.PerformanceTests;

internal class Program
{
	static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.WriteTo.Console(
				outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}",
				theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code
			)
			.CreateBootstrapLogger();

		var builder = Host.CreateApplicationBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog();

		builder.Services.AddSingleton<BinCollectionsTestDataProvider>(sp =>
		{
			return new BinCollectionsTestDataProvider();
		});
		builder.Services.AddSingleton<ORLibraryScenarioTestDataProvider>();
		builder.Services.AddSingleton<AlgorithmFamiliesCollection>();
		builder.Services.AddTransient<ITestsRunner, PackingEfficiencyTestsRunner>();
		builder.Services.AddTransient<ITestsRunner, PackingPerformanceTestsRunner>();

		IHost host = builder.Build();

		var packedBinVolumePercentages = new Dictionary<string, List<decimal>>();
		var maxPotentialPackingEfficiencyPercentages = new List<decimal>();

		using (var scope = host.Services.CreateScope())
		{
			var testsRunners = scope.ServiceProvider.GetServices<ITestsRunner>();
			var tasks = new List<Task<List<TestSummaryAction>>>();
			foreach ( var testRunner in testsRunners) 
			{
				tasks.Add(Task.Run(() => testRunner.Run()));
			}
			var testRunnersSummaries = await Task.WhenAll(tasks);

			foreach (var testRunnerSummaries in testRunnersSummaries)
			{
				foreach (var testSummary in testRunnerSummaries)
				{
					testSummary.Action();
				}
			}
		}
	}

}

