using Binacle.Lib.PerformanceTests.Models;
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
	    
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.PackingEfficiencyTests>(sp => new Tests.BischoffSuite.PackingEfficiencyTests(
		    file: new Models.ResultFile
		    {
			    Filename = "PackingEfficiency",
			    Title = "Packing Efficiency Tests",
			    Description = "Packing efficiency results for various algorithms using the Bischoff Suite scenarios"
		    },
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.PackingEfficiencyTests>>()
		));

	    
	    var regressionTestsFile = new Models.ResultFile
	    {
		    Filename = "RegressionTests",
		    Title = "Regression Tests",
		    Description = "Regression tests for various algorithms using the Bischoff Suite scenarios"
	    };
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.RegressionTests>(sp => new Tests.BischoffSuite.RegressionTests(
		    title: "FFD Regression Tests",
		    description: "Regression tests for First Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    file: regressionTestsFile,
		    algorithmsUnderTest: [AlgorithmFactories.FFD_v1, AlgorithmFactories.FFD_v2],
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.RegressionTests>>()
	    ));
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.RegressionTests>(sp => new Tests.BischoffSuite.RegressionTests(
		    title: "WFD Regression Tests",
		    description: "Regression tests for Worst Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    file: regressionTestsFile,
		    algorithmsUnderTest: [AlgorithmFactories.WFD_v1, AlgorithmFactories.WFD_v2],
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.RegressionTests>>()
	    ));
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.RegressionTests>(sp => new Tests.BischoffSuite.RegressionTests(
		    title: "BFD Regression Tests",
		    description: "Regression tests for Best Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    file: regressionTestsFile,
		    algorithmsUnderTest: [AlgorithmFactories.BFD_v1, AlgorithmFactories.BFD_v2],
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.RegressionTests>>()
	    ));
	    
	    var statisticsFile = new Models.ResultFile
	    {
		    Filename = "EfficiencyStaticstics",
		    Title = "Efficiency Statistics",
		    Description = "Efficiency statistics for various algorithms using the Bischoff Suite scenarios"
	    };
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.EfficiencyStatisticsTests>(sp => new Tests.BischoffSuite.EfficiencyStatisticsTests(
		    title: "FFD Bischoff Statistics Tests",
		    description: "Efficiency statistics tests for First Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    file: statisticsFile,
		    scenarioCollectionsProvider: sp.GetRequiredService<ScenarioCollectionsProvider>(),
		    algorithmUnderTest: AlgorithmFactories.FFD_v2,
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.EfficiencyStatisticsTests>>()
	    ));
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.EfficiencyStatisticsTests>(sp => new Tests.BischoffSuite.EfficiencyStatisticsTests(
		    title: "WFD Bischoff Statistics Tests",
		    description: "Efficiency statistics tests for Worst Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    file: statisticsFile,
		    scenarioCollectionsProvider: sp.GetRequiredService<ScenarioCollectionsProvider>(),
		    algorithmUnderTest: AlgorithmFactories.WFD_v2,
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.EfficiencyStatisticsTests>>()
	    ));
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.EfficiencyStatisticsTests>(sp => new Tests.BischoffSuite.EfficiencyStatisticsTests(
		    title: "BFD Bischoff Statistics Tests",
		    description: "Efficiency statistics tests for Best Fit Decreasing algorithms using the Bischoff Suite scenarios",
		    file: statisticsFile,
		    scenarioCollectionsProvider: sp.GetRequiredService<ScenarioCollectionsProvider>(),
		    algorithmUnderTest: AlgorithmFactories.BFD_v2,
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.EfficiencyStatisticsTests>>()
	    ));
	    
	    
	    builder.Services.AddTransient<ITest, Tests.BischoffSuite.BaselineComparisonTests>(sp => new Tests.BischoffSuite.BaselineComparisonTests(
		    title: "Baseline (BFD) Comparison Tests",
		    description: "Comparison of various algorithms against the Best Fit Decreasing algorithm using the Bischoff Suite scenarios",
		    file: new ResultFile()
		    {
			  Filename  = "BaselineComparisonTests",
			  Title = "Baseline Comparison Tests",
			  Description = "Comparison of various algorithms against the Best Fit Decreasing algorithm using the Bischoff Suite scenarios"
		    },
		    baselineAlgorithm: AlgorithmFactories.BFD_v2,
		    algorithmsUnderTest: [
			    AlgorithmFactories.FFD_v2,
			    AlgorithmFactories.WFD_v2
		    ],
		    sp.GetRequiredService<ILogger<Tests.BischoffSuite.BaselineComparisonTests>>()
	    ));
	    

	    IHost host = builder.Build();

	    using (var scope = host.Services.CreateScope())
	    {
		    var testCoordinator = scope.ServiceProvider.GetService<TestRunner>();
		    await testCoordinator!.RunAsync();
	    }
    }
}
