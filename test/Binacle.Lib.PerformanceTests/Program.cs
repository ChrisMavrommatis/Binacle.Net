using Binacle.Lib.PerformanceTests.Services;
using Binacle.Lib.PerformanceTests.Tests;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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
			    theme: AnsiConsoleTheme.Code
		    )
		    .CreateBootstrapLogger();

	    var builder = Host.CreateApplicationBuilder();
	    builder.Logging.ClearProviders();
	    builder.Logging.AddSerilog();
	    
	    builder.Services.AddSingleton<IFileWriter, MarkdownFileWriter>();
	    builder.Services.AddSingleton<ScenarioCollectionsProvider>();
	    builder.Services.AddTransient<TestRunner>();
	    
	    builder.Services.AddTransient<ITest, BischoffSuiteTests>();
	    
	    builder.Services.AddTransient<ITest, RegressionTests>(sp => new RegressionTests(
		    title: "FFD Regression Tests",
		    description: "Regression tests for First Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    filename: "FFD_Regression",
		    algorithmsUnderTest: [AlgorithmFactories.FFD_v1, AlgorithmFactories.FFD_v2],
		    sp.GetRequiredService<ILogger<RegressionTests>>()
	    ));
	    
	    builder.Services.AddTransient<ITest, RegressionTests>(sp => new RegressionTests(
		    title: "WFD Regression Tests",
		    description: "Regression tests for Worst Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    filename: "WFD_Regression",
		    algorithmsUnderTest: [AlgorithmFactories.WFD_v1, AlgorithmFactories.WFD_v2],
		    sp.GetRequiredService<ILogger<RegressionTests>>()
	    ));
	    
	    builder.Services.AddTransient<ITest, RegressionTests>(sp => new RegressionTests(
		    title: "BFD Regression Tests",
		    description: "Regression tests for Best Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    filename: "BFD_Regression",
		    algorithmsUnderTest: [AlgorithmFactories.BFD_v1, AlgorithmFactories.BFD_v2],
		    sp.GetRequiredService<ILogger<RegressionTests>>()
	    ));
	    
	    builder.Services.AddTransient<ITest, BischoffEfficiencyStatisticsTests>(sp => new BischoffEfficiencyStatisticsTests(
		    title: "FFD Bischoff Statistics Tests",
		    description: "Efficiency statistics tests for First Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    filename: "FFD_Statistics",
		    scenarioCollectionsProvider: sp.GetRequiredService<ScenarioCollectionsProvider>(),
		    algorithmUnderTest: AlgorithmFactories.FFD_v2,
		    sp.GetRequiredService<ILogger<RegressionTests>>()
	    ));
	    
	    builder.Services.AddTransient<ITest, BischoffEfficiencyStatisticsTests>(sp => new BischoffEfficiencyStatisticsTests(
		    title: "WFD Bischoff Statistics Tests",
		    description: "Efficiency statistics tests for Worst Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    filename: "WFD_Statistics",
		    scenarioCollectionsProvider: sp.GetRequiredService<ScenarioCollectionsProvider>(),
		    algorithmUnderTest: AlgorithmFactories.WFD_v2,
		    sp.GetRequiredService<ILogger<RegressionTests>>()
	    ));

	    
	    builder.Services.AddTransient<ITest, BischoffEfficiencyStatisticsTests>(sp => new BischoffEfficiencyStatisticsTests(
		    title: "BFD Bischoff Statistics Tests",
		    description: "Efficiency statistics tests for Best Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    filename: "BFD_Statistics",
		    scenarioCollectionsProvider: sp.GetRequiredService<ScenarioCollectionsProvider>(),
		    algorithmUnderTest: AlgorithmFactories.BFD_v2,
		    sp.GetRequiredService<ILogger<RegressionTests>>()
	    ));
	    

	    IHost host = builder.Build();

	    using (var scope = host.Services.CreateScope())
	    {
		    var testCoordinator = scope.ServiceProvider.GetService<TestRunner>();
		    await testCoordinator!.RunAsync();
	    }
    }
}
