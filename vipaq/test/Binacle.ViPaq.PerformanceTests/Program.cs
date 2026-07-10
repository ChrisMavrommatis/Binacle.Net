using Binacle.TestReporting;
using Binacle.ViPaq.Compression;
using Binacle.ViPaq.PerformanceTests.ExtensionMethods;
using Binacle.ViPaq.PerformanceTests.Tests;
using Binacle.ViPaq.TestsKernel.Models;
using Binacle.ViPaq.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Binacle.ViPaq.PerformanceTests;

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

		// Fail fast before the reports: every real scenario must round-trip in every mode, and the curated
		// benchmark picks must still resolve.
		RoundTripCheck.Run();

		var builder = Host.CreateApplicationBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog();

		// Reports land in the repo's results/vipaq folder, next to the committed baselines.
		var repositoryRootLocator = RepositoryRoot.Bind();
		var resultsDirectory = repositoryRootLocator.Find("results", "vipaq");
		builder.Services.AddSingleton<IFileWriter>(new MarkdownFileWriter(resultsDirectory));
		builder.Services.AddTransient<TestRunner>();
		builder.Services.AddVipaqProtobufSizeComparisonTests();
		builder.Services.AddCodecCompressionCrossoverTests();
		
		IHost host = builder.Build();

		using var scope = host.Services.CreateScope();
		var testRunner = scope.ServiceProvider.GetRequiredService<TestRunner>();
		await testRunner.RunAsync();
	}
}
