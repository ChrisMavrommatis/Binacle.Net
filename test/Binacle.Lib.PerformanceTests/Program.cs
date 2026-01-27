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

	    builder.Services.AddSingleton<BischoffSuiteDataProvider>();
	    builder.Services.AddSingleton<IFileWriter, MarkdownFileWriter>();
	    builder.Services.AddTransient<TestRunner>();
	    builder.Services.AddTransient<ITest, BischoffSuitePackingEfficiencyTest>();

	    IHost host = builder.Build();

	    using (var scope = host.Services.CreateScope())
	    {
		    var testCoordinator = scope.ServiceProvider.GetService<TestRunner>();
		    await testCoordinator!.RunAsync();
	    }
    }
}
