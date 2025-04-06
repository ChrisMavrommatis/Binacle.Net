using Binacle.Lib.PerformanceTests.Services;
using Binacle.Lib.PerformanceTests.Tests;
using Binacle.Net.TestsKernel.Data.Providers.PackingEddiciency;
using Binacle.Net.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Binacle.Lib.PerformanceTests;

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
		builder.Services.AddSingleton<MarkdownFileWriter>();
		builder.Services.AddSingleton<DistinctAlgorithmCollection>();
		builder.Services.AddSingleton<ScalingBenchmarkTestsDataProvider>();
		builder.Services.AddTransient<TestRunner>();
		builder.Services.AddTransient<ITest, PackingEfficiency>();
		builder.Services.AddTransient<ITest, PackingTime>();
		builder.Services.AddTransient<ITest, PackingEfficiencyComparison>();

		IHost host = builder.Build();

		using (var scope = host.Services.CreateScope())
		{
			var testCoordinator = scope.ServiceProvider.GetService<TestRunner>();
			await testCoordinator!.RunAsync();
		}
	}

}

